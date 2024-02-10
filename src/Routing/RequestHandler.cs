﻿// The Sisk Framework source code
// Copyright (c) 2023 PROJECT PRINCIPIUM
//
// The code below is licensed under the MIT license as
// of the date of its publication, available at
//
// File name:   RequestHandler.cs
// Repository:  https://github.com/sisk-http/core

using Sisk.Core.Http;

namespace Sisk.Core.Routing;

/// <summary>
/// Represents an abstract class which implements <see cref="IRequestHandler"/>.
/// </summary>
/// <definition>
/// public abstract class RequestHandler : IRequestHandler
/// </definition>
/// <type>
/// Class
/// </type>
public abstract class RequestHandler : IRequestHandler
{
    /// <inheritdoc/>
    /// <nodoc/>
    public virtual RequestHandlerExecutionMode ExecutionMode { get; init; } = RequestHandlerExecutionMode.BeforeResponse;

    /// <inheritdoc/>
    /// <nodoc/>
    public abstract HttpResponse? Execute(HttpRequest request, HttpContext context);

    /// <summary>
    /// Returns an null <see cref="HttpResponse"/> reference, which points to the next request handler or route action.
    /// </summary>
    /// <definition>
    /// public HttpResponse? Next()
    /// </definition>
    /// <type>
    /// Class
    /// </type>
    public HttpResponse? Next()
    {
        return null;
    }
}
