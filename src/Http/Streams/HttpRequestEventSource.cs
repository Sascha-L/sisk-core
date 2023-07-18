﻿using System.Net;

namespace Sisk.Core.Http.Streams
{
    /// <summary>
    /// An <see cref="HttpRequestEventSource"/> instance opens a persistent connection to the request, which sends events in text/event-stream format.
    /// </summary>
    /// <definition>
    /// public class HttpRequestEventSource : IDisposable
    /// </definition>
    /// <type>
    /// Class 
    /// </type>
    public class HttpRequestEventSource : IDisposable
    {
        ManualResetEvent terminatingMutex = new ManualResetEvent(false);
        HttpStreamPingPolicy pingPolicy;
        HttpListenerResponse res;
        HttpListenerRequest req;
        HttpRequest reqObj;
        HttpServer hostServer;
        TimeSpan keepAlive = TimeSpan.Zero;
        DateTime lastSuccessfullMessage = DateTime.Now;
        int length = 0;

        internal List<string> sendQueue = new List<string>();
        internal bool hasSentData = false;

        // 
        // isClosed determines if this instance has some connection or not
        // isDisposed determines if this object was removed from their collection but wasnt collected by gc yet
        // 

        private bool isClosed = false;
        private bool isDisposed = false;

        /// <summary>
        /// Gets the <see cref="HttpStreamPingPolicy"/> for this HTTP event source connection.
        /// </summary>
        /// <definition>
        /// public HttpStreamPingPolicy PingPolicy { get; }
        /// </definition>
        /// <type>
        /// Property
        /// </type>
        public HttpStreamPingPolicy PingPolicy { get => pingPolicy; }

        /// <summary>
        /// Gets the <see cref="Http.HttpRequest"/> object which created this Event Source instance.
        /// </summary>
        /// <definition>
        /// public HttpRequest HttpRequest { get; }
        /// </definition>
        /// <type>
        /// Property
        /// </type>
        public HttpRequest HttpRequest => reqObj;

        /// <summary>
        /// Gets an integer indicating the total bytes sent by this instance to the client.
        /// </summary>
        /// <definition>
        /// public int SentContentLength { get; }
        /// </definition>
        /// <type>
        /// Property
        /// </type>
        public int SentContentLength { get => length; }

        /// <summary>
        /// Gets an unique identifier label to this EventStream connection, useful for finding this connection's reference later.
        /// </summary>
        /// <definition>
        /// public string? Identifier { get; }
        /// </definition>
        /// <type>
        /// Property
        /// </type>
        public string? Identifier { get; private set; }

        /// <summary>
        /// Gets an boolean indicating if this connection is open and this instance can send messages.
        /// </summary>
        /// <definition>
        /// public bool IsDisposed { get; }
        /// </definition>
        /// <type>
        /// Property
        /// </type>
        public bool IsActive { get; private set; }

        internal HttpRequestEventSource(string? identifier, HttpListenerResponse res, HttpListenerRequest req, HttpRequest host)
        {
            this.res = res ?? throw new ArgumentNullException(nameof(res));
            this.req = req ?? throw new ArgumentNullException(nameof(req));
            Identifier = identifier;
            hostServer = host.baseServer;
            reqObj = host;
            pingPolicy = new HttpStreamPingPolicy(this);

            hostServer._eventCollection.RegisterEventSource(this);

            IsActive = true;

            res.AddHeader("Cache-Control", "no-store, no-cache");
            res.AddHeader("Content-Type", "text/event-stream");
            res.AddHeader("X-Powered-By", HttpServer.poweredByHeader);

            HttpServer.SetCorsHeaders(host.baseServer.ServerConfiguration.Flags, req, host.hostContext.CrossOriginResourceSharingPolicy, res);
        }

        private void keepAliveTask()
        {
            while (IsActive)
            {
                if (lastSuccessfullMessage < DateTime.Now - keepAlive)
                {
                    Dispose();
                    break;
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
        }

        /// <summary>
        /// Configures the ping policy for this instance of HTTP Event Source.
        /// </summary>
        /// <param name="act">The method that runs on the ping policy for this HTTP Event Source.</param>
        /// <definition>
        /// public void WithPing(Action'HttpStreamPingPolicy act)
        /// </definition>
        /// <type>
        /// Method
        /// </type>
        public void WithPing(Action<HttpStreamPingPolicy> act)
        {
            act(pingPolicy);
        }

        /// <summary>
        /// Sends an header to the streaming context.
        /// </summary>
        /// <param name="name">The header name.</param>
        /// <param name="value">The header value.</param>
        /// <definition>
        /// public void AppendHeader(string name, string value)
        /// </definition>
        /// <type>
        /// Method
        /// </type>
        public void AppendHeader(string name, string value)
        {
            if (hasSentData)
            {
                throw new InvalidOperationException("It's not possible to set headers after a message has been sent in this instance.");
            }
            res.AddHeader(name, value);
        }

        /// <summary>
        /// Writes a event message with their data to the event listener and returns an boolean indicating if the message was delivered to the client.
        /// </summary>
        /// <param name="data">The message text.</param>
        /// <definition>
        /// public bool Send(string data)
        /// </definition>
        /// <type>
        /// Method
        /// </type>
        public bool Send(string data)
        {
            if (!IsActive)
            {
                return false;
            }
            hasSentData = true;
            sendQueue.Add($"data: {data}\n\n");
            Flush();
            return true;
        }

        /// <summary>
        /// Writes a event message with their data to the event listener and returns an boolean indicating if the message was delivered to the client.
        /// </summary>
        /// <param name="data">The message object.</param>
        /// <definition>
        /// public bool Send(object? data)
        /// </definition>
        /// <type>
        /// Method
        /// </type>
        public bool Send(object? data)
        {
            if (!IsActive)
            {
                return false;
            }
            hasSentData = true;
            sendQueue.Add($"data: {data?.ToString()}\n\n");
            Flush();
            return true;
        }

        /// <summary>
        /// Asynchronously waits for the connection to close before continuing execution. This method
        /// is released when either the client or the server reaches an sending failure.
        /// </summary>
        /// <definition>
        /// public void KeepAlive()
        /// </definition>
        /// <type>
        /// Method
        /// </type>
        public void KeepAlive()
        {
            if (!IsActive)
            {
                throw new InvalidOperationException("Cannot keep alive an instance that has it's connection disposed.");
            }
            terminatingMutex.WaitOne();
        }

        /// <summary>
        /// Asynchronously waits for the connection to close before continuing execution with
        /// an maximum keep alive timeout. This method is released when either the client or the server reaches an sending failure.
        /// </summary>
        /// <param name="maximumIdleTolerance">The maximum timeout interval for an idle connection to automatically release this method.</param>
        /// <definition>
        /// public void KeepAlive()
        /// </definition>
        /// <type>
        /// Method
        /// </type>
        public void WaitForFail(TimeSpan maximumIdleTolerance)
        {
            if (!IsActive)
            {
                throw new InvalidOperationException("Cannot keep alive an instance that has it's connection disposed.");
            }
            keepAlive = maximumIdleTolerance;
            new Task(keepAliveTask).Start();
            terminatingMutex.WaitOne();
        }

        /// <summary>
        /// Closes the event listener and it's connection.
        /// </summary>
        /// <definition>
        /// public HttpResponse Close()
        /// </definition>
        /// <type>
        /// Method
        /// </type>
        public HttpResponse Close()
        {
            if (!isClosed)
            {
                isClosed = true;
                Flush();
                Dispose();
                hostServer._eventCollection.UnregisterEventSource(this);
            }
            return new HttpResponse(HttpResponse.HTTPRESPONSE_SERVER_CLOSE)
            {
                CalculedLength = length
            };
        }

        /// <summary>
        /// Cancels the sending queue from sending pending messages and clears the queue.
        /// </summary>
        /// <definition>
        /// public void Cancel()
        /// </definition>
        /// <type>
        /// Method
        /// </type>
        public void Cancel()
        {
            sendQueue.Clear();
        }

        internal void Flush()
        {
            for (int i = 0; i < sendQueue.Count; i++)
            {
                if (isClosed)
                {
                    return;
                }
                string item = sendQueue[i];
                byte[] itemBytes = req.ContentEncoding.GetBytes(item);
                try
                {
                    res.OutputStream.Write(itemBytes);
                    length += itemBytes.Length;
                    sendQueue.RemoveAt(0);
                    lastSuccessfullMessage = DateTime.Now;
                }
                catch (Exception)
                {
                    Dispose();
                }
            }
        }

        /// <summary>
        /// Flushes and releases the used resources of this class instance.
        /// </summary>
        /// <definition>
        /// public void Dispose()
        /// </definition>
        /// <type>
        /// Method 
        /// </type>
        public void Dispose()
        {
            if (isDisposed) return;
            Close();
            sendQueue.Clear();
            terminatingMutex.Set();
            IsActive = false;
            isDisposed = true;
        }
    }
}
