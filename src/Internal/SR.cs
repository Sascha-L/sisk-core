﻿// The Sisk Framework source code
// Copyright (c) 2023 PROJECT PRINCIPIUM
//
// The code below is licensed under the MIT license as
// of the date of its publication, available at
//
// File name:   SR.cs
// Repository:  https://github.com/sisk-http/core

namespace Sisk.Core.Internal;

static class SR
{
    public const string MultipartObject_ContentTypeMissing = "Content-Type header cannot be null when retriving a multipart form content";
    public const string MultipartObject_BoundaryMissing = "No boundary was specified for this multipart form content.";
    public const string MultipartObject_EmptyFieldName = "Content-part object position {0} cannot have an empty field name.";

    public const string HttpContextBagRepository_UndefinedDynamicProperty = "The specified type {0} was not defined in this context bag repository.";

    public const string HttpRequest_InvalidForwardedIpAddress = "The forwarded IP address is invalid.";
    public const string HttpRequest_InvalidCookieSyntax = "The cookie header is invalid or is it has an malformed syntax.";
    public const string HttpRequest_GetQueryValue_CastException = "Cannot cast the query item {0} value into an {1}.";
    public const string HttpRequest_SendTo_MaxRedirects = "Too many internal route redirections.";
    public const string HttpRequest_InputStreamAlreadyLoaded = "Unable to read InputStream as it has already been read.";
    public const string HttpRequest_AlreadyInStreamingState = "This HTTP request is already streaming another context.";

    public const string HttpResponse_Redirect_NotMatchGet = "The specified method does not handle GET requests.";

    public const string Httpserver_NoListeningHost = "Cannot start the HTTP server with no listening hosts.";
    public const string Httpserver_StartMessage = "The HTTP server is listening at {0}";
    public const string Httpserver_Commons_HeaderAfterContents = "Cannot send headers or status code after sending response contents.";
    public const string Httpserver_Commons_RouterTimeout = "Request maximum execution time exceeded it's limit.";

    public const string HttpStatusCode_IllegalStatusCode = "The HTTP status code must be three-digits long.";
    public const string HttpStatusCode_IllegalStatusReason = "The HTTP reason phrase must be equal or smaller than 8192 characters.";

    public const string ListeningHostRepository_Duplicate = "This ListeningHost has already been defined in this collection with identical definitions.";

    public const string ListeningPort_Parser_UndefinedScheme = "Scheme was not defined in the URI.";
    public const string ListeningPort_Parser_UndefinedPort = "The URI port must be explicitly defined.";
    public const string ListeningPort_Parser_UriNotTerminatedSlash = "The URI must terminate with /.";
    public const string ListeningPort_Parser_InvalidScheme = "The URI scheme must be http or https.";
    public const string ListeningPort_Parser_InvalidPort = "The URI port is invalid.";

    public const string LogStream_Peek_NotFilePath = "This method only works when the LogStream is appending contents to an file.";
    public const string LogStream_ExceptionDump_Header = "Exception thrown at {0}";
    public const string LogStream_ExceptionDump_TrimmedFooter = " + ... other trimmed inner exceptions";
    public const string LogStream_RotatingLogPolicy_NotLocalFile = "Cannot link an rotaging log policy to an log stream which ins't pointing to an local file.";

    public const string HttpRequestEventSource_KeepAliveDisposed = "Cannot keep alive an instance that has it's connection disposed.";

    public const string Router_AutoScanModules_RequiresUnreferencedCode = "This method needs to search for types in your assembly, which can be trimmed in an AOT compilation.";
    public const string Router_AutoScanModules_TModuleSameAssembly = "When T is RouterModule and an input assembly is not specified, no routes will be added to the router.";
    public const string Router_Set_Collision = "A possible route collision could happen between route {0} and route {1}. Please review the methods and paths of these routes.";
    public const string Router_Set_Exception = "Couldn't set method {0}.{1} as an route. See inner exception.";
    public const string Router_Set_InvalidRouteStart = "Route paths must start with /.";
    public const string Router_RouteDefinitionNotFound = "No route definition was found for the given action. It may be possible that the informed method does not implement the RouteAttribute attribute.";
    public const string Router_Handler_HttpResponseRegister = "Cannot register HttpResponse as an valid type to the action handler.";
    public const string Router_Handler_Duplicate = "The specified type is already defined in this router instance.";
    public const string Router_Handler_ActionNullValue = "Action result values cannot be null values.";
    public const string Router_Handler_UnrecognizedAction = "Action of type \"{0}\" doens't have an action handler registered on the router that issued it.";
    public const string Router_NotBinded = "No HTTP server instance is binded to this Router.";
    public const string Router_NoRouteActionDefined = "No route action was defined to the route {0}.";

    public const string Route_Action_ValueTypeSet = "Defining actions which their return type is an value type is not supported. Encapsulate it with ValueResult<T>.";
    public const string Route_Action_AsyncMissingGenericType = "Async route {0} action must return an object in addition to Task.";

    public const string ValueResult_Null = "ValueResult cannot hold null values.";

    public const string Provider_ConfigParser_ConfigFileNotFound = "Configuration file {0} was not found.";
    public const string Provider_ConfigParser_ConfigFileInvalid = "Couldn't read the configuration file.";
    public const string Provider_ConfigParser_NoListeningHost = "When defined the ListeningHost, the configuration file must define at least one listening host port.";
    public const string Provider_ConfigParser_SectionRequired = "The \"{0}\" section in the configuration file is required.";
    public const string Provider_ConfigParser_CaughtException = "Unable to configure the Http server. See details below:";

    public const string InitializationParameterCollection_NullOrEmptyParameter = "The required parameter \"{0}\" is either empty or not present in the configuration file.";
    public const string InitializationParameterCollection_NullParameter = "The required parameter \"{0}\" is not present in the configuration file.";
    public const string InitializationParameterCollection_MapCastException = "Cannot cast the value \"{0}\" into an {1}.";
}