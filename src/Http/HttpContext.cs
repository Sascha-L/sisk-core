﻿// The Sisk Framework source code
// Copyright (c) 2024 PROJECT PRINCIPIUM
//
// The code below is licensed under the MIT license as
// of the date of its publication, available at
//
// File name:   HttpContext.cs
// Repository:  https://github.com/sisk-http/core

using Sisk.Core.Entity;
using Sisk.Core.Routing;
using System.Collections.Specialized;

namespace Sisk.Core.Http
{
    /// <summary>
    /// Represents an context that is shared in a entire HTTP session.
    /// </summary>
    public sealed class HttpContext
    {
        /// <summary>
        /// Gets or sets an <see cref="NameValueCollection"/> indicating HTTP headers which
        /// will overwrite headers set by CORS, router response or request handlers.
        /// </summary>
        public HttpHeaderCollection OverrideHeaders { get; set; } = new HttpHeaderCollection();

        /// <summary>
        /// Gets the <see cref="ListeningHost"/> instance of this HTTP context.
        /// </summary>
        public ListeningHost ListeningHost { get; private set; }

        /// <summary>
        /// Gets or sets a managed object that is accessed and modified by request handlers.
        /// </summary>
        public TypedValueDictionary RequestBag { get; set; } = new TypedValueDictionary();

        /// <summary>
        /// Gets the context HTTP Server instance.
        /// </summary>
        public HttpServer HttpServer { get; private set; }

        /// <summary>
        /// Gets or sets the HTTP response for this context. This property is only not null when a post-executing <see cref="IRequestHandler"/> was executed for this router context.
        /// </summary>
        public HttpResponse? RouterResponse { get; internal set; } = null!;

        /// <summary>
        /// Gets the HTTP request which is contained in this HTTP context.
        /// </summary>
        public HttpRequest Request { get; private set; }

        /// <summary>
        /// Gets the matched HTTP Route object from the Router.
        /// </summary>
        public Route? MatchedRoute { get; internal set; }

        /// <summary>
        /// Gets the <see cref="Sisk.Core.Routing.Router"/> where this context was
        /// created.
        /// </summary>
        public Router? Router { get; internal set; }

        internal HttpContext(HttpServer httpServer, HttpRequest request, Route? matchedRoute, ListeningHost host)
        {
            this.Request = request;
            this.HttpServer = httpServer;
            this.MatchedRoute = matchedRoute;
            this.ListeningHost = host;
        }
    }
}
