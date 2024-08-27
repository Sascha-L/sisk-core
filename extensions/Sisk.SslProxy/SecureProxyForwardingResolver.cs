﻿// The Sisk Framework source code
// Copyright (c) 2023 PROJECT PRINCIPIUM
//
// The code below is licensed under the MIT license as
// of the date of its publication, available at
//
// File name:   SecureProxyForwardingResolver.cs
// Repository:  https://github.com/sisk-http/core

using Sisk.Core.Http;
using Sisk.Ssl;
using System.Net;

namespace Sisk.SslProxy;

public class SecureProxyForwardingResolver : ForwardingResolver
{
    /// <inheritdoc/>
    public override IPAddress OnResolveClientAddress(HttpRequest request, IPEndPoint connectingEndpoint)
    {
        var digestHeader = request.Headers[Constants.XDigestHeaderName];
        var clientIpHeader = request.Headers[Constants.XClientIpHeaderName];

        if (string.Compare(digestHeader, SecureProxy.ProxyDigest, true) == 0 && clientIpHeader is not null)
        {
            return IPAddress.Parse(clientIpHeader);
        } 
        
        throw new InvalidOperationException("The incoming request ins't trusted by the proxy.");
    }
}
