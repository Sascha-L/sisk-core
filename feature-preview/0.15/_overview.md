## Sisk 0.15 features release overview:

| Feature | Description | Status |
| - | - | - |
| Stream ping | This proposal allows sending periodic packets automatically in web sockets or server sent events. | Approved |
| Implicit return types | Implicitly typed returns allow callbacks to return any object other than HttpResponse. | Approved |
| Overridable headers | Overloadable headers is a way to ensure that these headers are sent at the end of the response. | Approved |
| Dynamic bag | This proposal lets you store objects in the `HttpContext.Bag` without the need to pack/unpack or define a key for them. | Testing | 
| Input streams | Allows the user to read the input content stream inline. | Testing |
| Sessions | Sessions are a way to track and identify a user throughout the application and take control of session storage on the server. | Testing |
| Fluent responses | Allows the user to take advantage of the Fluent Interface for HttpResponse objects and create simpler responses. | Testing |

## Sisk 0.15 changelog

- [a81ca09](https://github.com/sisk-http/core/commit/a81ca09866cdb44c98d0c34336d91f80de8fb2c0)
    - Added properties for SessionConfiguration: HttpOnly and DisposeOnBrowserClose. Names can change in future changes.
    - Added Fluent Interface style methods for HttpResponse: WithContent, WithHeader, WithStatus, not yet documented.
    - Added the Session.Get() and Session.Set() methods, which has functionality similar to HttpRequest.Get/SetContextBag().
    - Cookies set by the HTTP server are now sent with the expiration parameter, which is defined in ISessionController.SessionExpirity.
    - RouteMethod.Any nows flags all the previous defined Route methods.
    - ISessionController now requires SessionExpirity to be defined.
    - Extended the CreateRedirectResponse method to be able to extract the route from a RouteCallback with the RouteAttribute attribute.
    - Fixed inconsistencies in the Sisk.ServiceProvider package namespace. Now the default namespace is Sisk.ServiceProvider.
    - Fixed a bug where requests with non-common HTTP methods were not accepted in routes even when used in RouteMethod.Any.
    - Dropped System.Runtime.Caching dependency.

- [7d6330d](https://github.com/sisk-http/core/commit/7d6330dba06489563a8d40044bbb1f031039581e)
    - Added the Sisk.BasicAuth package source code at /extensions.
    - Migrated the Sisk.ServiceProvider package source to /extensions.
    - Added the HttpContext.Session property.
    - Added the HttpServerConfiguration.SessionConfiguration property.
    - Added the HttpServerFlags.SessionIdCookie field.
    - Added the sessions source code into repository.
    - The HttpContext.HttpServer property ins't nullable anymore.
    - The HttpRequest.Context property inst't nullable anymore.

- [30de03c](https://github.com/sisk-http/core/commit/30de03cdb9df577039d267d14e94016f71cac656)
    - Added the HttpRequest.InputStream property. [Spec.](https://github.com/sisk-http/core/blob/main/feature-preview/0.15/input-stream.md)

- [a445ad](https://github.com/sisk-http/core/commit/a445ad3651f910b3fbc6b8cb98ee08290d2410e4)
    - Added the HttpContext.OverrideHeaders property.
    - Added the HttpRequest.SetContextBag and GetContextBag methods.
    - Added the Router.RegisterValueHandler method.
    - Added the RouterModule class.
    - Added the ValueResult class.
    - Rewrite the return type of HttpRequest.SendTo from HttpResponse to object.
    - Rewrite the LogStream.WriteException output format.
    - Rewrite the return type of RouterCallback delegate from HttpResponse to object.
    - Simplified the way HttpRequest obtains the origin IP of the request.

- [bbb0e84](https://github.com/sisk-http/core/commit/bbb0e84046eeb8684393230dfc4a4baacb062fba), [6f77f3d](https://github.com/sisk-http/core/commit/6f77f3db71fcdbe435d61db6fe7914f7ecb2ec06), [d643d71](https://github.com/sisk-http/core/commit/d643d718f256e6d1a3df276ac28dced09a5ec627)
    - Added an string representation to HttpRequest.ToString().
    - Created the HttpStreamPingPolicy class.
    - Renamed HttpRequestEventSource.KeepAlive -> WaitForFail.
    - Added the HttpWebSocket.MaxAttempts property.
    - Fixed an bug where the WebSocket was throwing an exception when the client didn't terminated the close handshake with the server.