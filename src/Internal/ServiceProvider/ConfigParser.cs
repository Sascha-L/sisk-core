﻿// The Sisk Framework source code
// Copyright (c) 2023 PROJECT PRINCIPIUM
//
// The code below is licensed under the MIT license as
// of the date of its publication, available at
//
// File name:   ConfigParser.cs
// Repository:  https://github.com/sisk-http/core

using Sisk.Core.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Sisk.Core.Internal.ServiceProvider
{
    internal class ConfigParser
    {
        internal static void ParseConfiguration(ProviderContext prov)
        {
            string filename = Path.GetFullPath(prov.ConfigurationFile);
            if (!File.Exists(filename))
            {
                if (prov.CreateConfigurationFileIfDoenstExists)
                {
                    File.Create(filename).Close();
                }
                throw new ArgumentException(string.Format(SR.Provider_ConfigParser_ConfigFileNotFound, prov.ConfigurationFile));
            }

            string fileContents = File.ReadAllText(filename);
            ConfigStructureFile? config = System.Text.Json.JsonSerializer.Deserialize(fileContents, typeof(ConfigStructureFile),
                new SourceGenerationContext(new System.Text.Json.JsonSerializerOptions()
                {
                    AllowTrailingCommas = true,
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip
                })) as ConfigStructureFile;

            if (config is null)
            {
                throw new Exception(SR.Provider_ConfigParser_ConfigFileInvalid);
            }

            if (config.Server == null && prov._requiredSections.HasFlag(Http.Hosting.PortableConfigurationRequireSection.ServerConfiguration))
                throw new Exception(string.Format(SR.Provider_ConfigParser_SectionRequired, nameof(config.Server)));
            if (config.ListeningHost == null && prov._requiredSections.HasFlag(Http.Hosting.PortableConfigurationRequireSection.ListeningHost))
                throw new Exception(string.Format(SR.Provider_ConfigParser_SectionRequired, nameof(config.ListeningHost)));
            if (config.Parameters == null && prov._requiredSections.HasFlag(Http.Hosting.PortableConfigurationRequireSection.Parameters))
                throw new Exception(string.Format(SR.Provider_ConfigParser_SectionRequired, nameof(config.Parameters)));

            if (config.Server != null)
            {
                prov.Host.ServerConfiguration.ResolveForwardedOriginAddress = config.Server.ResolveForwardedOriginAddress;
                prov.Host.ServerConfiguration.ResolveForwardedOriginHost = config.Server.ResolveForwardedOriginHost;
                prov.Host.ServerConfiguration.DefaultEncoding = Encoding.GetEncoding(config.Server.DefaultEncoding);
                prov.Host.ServerConfiguration.MaximumContentLength = config.Server.MaximumContentLength;
                prov.Host.ServerConfiguration.IncludeRequestIdHeader = config.Server.IncludeRequestIdHeader;
                prov.Host.ServerConfiguration.ThrowExceptions = config.Server.ThrowExceptions;

                if (config.Server.AccessLogsStream?.ToLower() == "console")
                {
                    prov.Host.ServerConfiguration.AccessLogsStream = LogStream.ConsoleOutput;
                }
                else if (config.Server.AccessLogsStream != null)
                {
                    prov.Host.ServerConfiguration.AccessLogsStream = new LogStream(config.Server.AccessLogsStream);
                }
                else
                {
                    prov.Host.ServerConfiguration.AccessLogsStream = null;
                }

                if (config.Server.ErrorsLogsStream?.ToLower() == "console")
                {
                    prov.Host.ServerConfiguration.ErrorsLogsStream = LogStream.ConsoleOutput;
                }
                else if (config.Server.ErrorsLogsStream != null)
                {
                    prov.Host.ServerConfiguration.ErrorsLogsStream = new LogStream(config.Server.ErrorsLogsStream);
                }
                else
                {
                    prov.Host.ServerConfiguration.ErrorsLogsStream = null;
                }
            }

            if (config.Parameters != null)
            {
                foreach (var prop in config.Parameters)
                {
                    prov.Host.Parameters.Add(prop.Key, prop.Value?.AsValue().GetValue<object>().ToString());
                }
                prov.Host.Parameters.MakeReadonly();
            }

            if (config.ListeningHost != null)
            {
                if (config.ListeningHost.Ports is null || config.ListeningHost.Ports.Length == 0)
                {
                    throw new InvalidOperationException(SR.Provider_ConfigParser_NoListeningHost);
                }

                ListeningHost host = prov.TargetListeningHost;

                host.Label = config.ListeningHost.Label;
                host.Ports = config.ListeningHost.Ports.Select(s => new ListeningPort(s)).ToArray();

                if (config.ListeningHost.CrossOriginResourceSharingPolicy?.MaxAge != null)
                    host.CrossOriginResourceSharingPolicy.MaxAge = TimeSpan.FromSeconds((double)config.ListeningHost.CrossOriginResourceSharingPolicy.MaxAge);
                if (config.ListeningHost.CrossOriginResourceSharingPolicy?.AllowOrigin != null)
                    host.CrossOriginResourceSharingPolicy.AllowOrigin = config.ListeningHost.CrossOriginResourceSharingPolicy.AllowOrigin;
                if (config.ListeningHost.CrossOriginResourceSharingPolicy?.AllowOrigins != null)
                    host.CrossOriginResourceSharingPolicy.AllowOrigins = config.ListeningHost.CrossOriginResourceSharingPolicy.AllowOrigins;
                if (config.ListeningHost.CrossOriginResourceSharingPolicy?.AllowMethods != null)
                    host.CrossOriginResourceSharingPolicy.AllowMethods = config.ListeningHost.CrossOriginResourceSharingPolicy.AllowMethods;
                if (config.ListeningHost.CrossOriginResourceSharingPolicy?.AllowCredentials != null)
                    host.CrossOriginResourceSharingPolicy.AllowCredentials = config.ListeningHost.CrossOriginResourceSharingPolicy.AllowCredentials;
                if (config.ListeningHost.CrossOriginResourceSharingPolicy?.AllowHeaders != null)
                    host.CrossOriginResourceSharingPolicy.AllowHeaders = config.ListeningHost.CrossOriginResourceSharingPolicy.AllowHeaders;
                if (config.ListeningHost.CrossOriginResourceSharingPolicy?.ExposeHeaders != null)
                    host.CrossOriginResourceSharingPolicy.ExposeHeaders = config.ListeningHost.CrossOriginResourceSharingPolicy.ExposeHeaders;
            }
        }
    }

    [JsonSerializable(typeof(ConfigStructureFile))]
    internal class ConfigStructureFile
    {
        public ConfigStructureFile__ServerConfiguration? Server { get; set; } = null!;
        public ConfigStructureFile__ListeningHost? ListeningHost { get; set; } = null!;
        public JsonObject? Parameters { get; set; }
    }

    [JsonSerializable(typeof(ConfigStructureFile__ServerConfiguration))]
    internal class ConfigStructureFile__ServerConfiguration
    {
        public string? AccessLogsStream { get; set; } = "console";
        public string? ErrorsLogsStream { get; set; }
        public bool ResolveForwardedOriginAddress { get; set; } = false;
        public bool ResolveForwardedOriginHost { get; set; } = false;
        public string DefaultEncoding { get; set; } = "UTF-8";
        public long MaximumContentLength { get; set; } = 0;
        public bool IncludeRequestIdHeader { get; set; } = false;
        public bool ThrowExceptions { get; set; } = true;
    }

    [JsonSerializable(typeof(ConfigStructureFile__ListeningHost__CrossOriginResourceSharingPolicy))]
    internal class ConfigStructureFile__ListeningHost__CrossOriginResourceSharingPolicy
    {
        public bool? AllowCredentials { get; set; } = null;
        public string[]? ExposeHeaders { get; set; }
        public string? AllowOrigin { get; set; }
        public string[]? AllowOrigins { get; set; }
        public string[]? AllowMethods { get; set; }
        public string[]? AllowHeaders { get; set; }
        public int? MaxAge { get; set; } = null;
    }

    [JsonSerializable(typeof(ConfigStructureFile__ListeningHost))]
    internal class ConfigStructureFile__ListeningHost
    {
        public string? Label { get; set; }
        public string[]? Ports { get; set; }

        public ConfigStructureFile__ListeningHost__CrossOriginResourceSharingPolicy? CrossOriginResourceSharingPolicy { get; set; }
    }

    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(ConfigStructureFile))]
    internal partial class SourceGenerationContext : JsonSerializerContext
    {
    }
}
