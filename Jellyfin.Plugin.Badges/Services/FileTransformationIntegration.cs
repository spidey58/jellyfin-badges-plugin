using System;
using System.Linq;
using System.Runtime.Loader;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Jellyfin.Plugin.Badges.Services
{
    /// <summary>
    /// Integrates with the community "File Transformation" plugin
    /// (IAmParadox27/jellyfin-plugin-file-transformation) to inject our
    /// script tag into index.html at request time, in memory, without
    /// ever writing to disk.
    /// </summary>
    public static class FileTransformationIntegration
    {
        private const string InjectedScriptTag =
            "<script defer src=\"Badges/script\"></script>";

        public static bool TryRegister(ILogger logger)
        {
            try
            {
                var fileTransformationAssembly = AssemblyLoadContext.All
                    .SelectMany(ctx => ctx.Assemblies)
                    .FirstOrDefault(asm => asm.FullName?.Contains(".FileTransformation", StringComparison.OrdinalIgnoreCase) ?? false);

                if (fileTransformationAssembly is null)
                {
                    logger.LogInformation(
                        "Jellyfin Badges: File Transformation plugin not found, will fall back to direct file write");
                    return false;
                }

                var pluginInterfaceType = fileTransformationAssembly.GetType(
                    "Jellyfin.Plugin.FileTransformation.PluginInterface");

                var registerMethod = pluginInterfaceType?.GetMethod("RegisterTransformation");

                if (registerMethod is null)
                {
                    logger.LogWarning(
                        "Jellyfin Badges: found File Transformation assembly but could not locate RegisterTransformation method");
                    return false;
                }

                var payload = new JObject
                {
                    ["id"] = "2c0f75c3-6aad-4003-95e3-0fd418279ac3",
                    ["fileNamePattern"] = "index\\.html$",
                    ["callbackAssembly"] = typeof(FileTransformationIntegration).Assembly.FullName,
                    ["callbackClass"] = typeof(FileTransformationIntegration).FullName,
                    ["callbackMethod"] = nameof(TransformIndexHtml)
                };

                registerMethod.Invoke(null, new object?[] { payload });

                logger.LogInformation(
                    "Jellyfin Badges: successfully registered transformation with File Transformation plugin");
                return true;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Jellyfin Badges: File Transformation registration failed, will fall back to direct file write");
                return false;
            }
        }

        public static string TransformIndexHtml(object payload)
        {
            var contentsProperty = payload.GetType().GetProperty("contents");
            var contents = contentsProperty?.GetValue(payload)?.ToString() ?? string.Empty;

            if (string.IsNullOrEmpty(contents) || contents.Contains(InjectedScriptTag, StringComparison.Ordinal))
            {
                return contents;
            }

            const string HeadCloseTag = "</head>";
            var headIndex = contents.IndexOf(HeadCloseTag, StringComparison.OrdinalIgnoreCase);

            if (headIndex < 0)
            {
                return contents;
            }

            return contents.Insert(headIndex, InjectedScriptTag + "\n");
        }
    }
}
