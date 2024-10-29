﻿using Asmichi.ProcessManagement;
using Sisk.Core.Http.Hosting;
using Sisk.Helpers.Mitmproxy;

namespace Sisk.Helpers.mitmproxy;

public static class MitmproxyHelper
{
    public static HttpServerHostContextBuilder UseMitmproxy(this HttpServerHostContextBuilder builder) => UseMitmproxy(builder, proxyPort: 0);

    public static HttpServerHostContextBuilder UseMitmproxy(this HttpServerHostContextBuilder builder,
        ushort proxyPort = 0,
        bool silent = false,
        Action<ChildProcessStartInfo>? setupAction = null)
    {
        var proxy = new MitmproxyProvider(proxyPort, setupAction);
        proxy.Silent = silent;

        builder.UseHandler(proxy);
        builder.UseStartupMessage(() => $"""
            The mitmproxy is listening at:
            - https://localhost:{proxy.ProxyPort}/
            """);

        return builder;
    }
}