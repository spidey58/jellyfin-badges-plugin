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

        // Captured at registration time so the static callback (which File
        // Transformation invokes with only a payload argument, no DI) can
        // still log. This is the only way to verify from server logs
        // whether the callback is actually being invoked per-request, as
        // opposed to registration merely succeeding.
        private static ILogger? _logger;

        public static bool TryRegister(ILogger logger)
        {
            _logger = logger;

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

                // Plain "index.html" (no regex escaping/anchors) - this
                // matches correctly whether fileNamePattern is parsed as a
                // full regex or a simpler substring/EndsWith check, since
                // an unescaped "." in regex still matches a literal "."
                var payload = new JObject
                {
                    ["id"] = "2c0f75c3-6aad-4003-95e3-0fd418279ac3",
                    ["fileNamePattern"] = "index.html",
                    ["callbackAssembly"] = typeof(FileTransformationIntegration).Assembly.FullName,
                    ["callbackClass"] = typeof(FileTransformationIntegration).FullName,
                    ["callbackMethod"] = nameof(TransformIndexHtml)
                };

                logger.LogInformation(
                    "Jellyfin Badges: registering with File Transformation - assembly={Assembly}, class={Class}, method={Method}",
                    payload["callbackAssembly"],
                    payload["callbackClass"],
                    payload["callbackMethod"]);

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
            _logger?.LogInformation("Jellyfin Badges: TransformIndexHtml callback invoked");

            try
            {
                var contentsProperty = payload.GetType().GetProperty("contents");
                var contents = contentsProperty?.GetValue(payload)?.ToString() ?? string.Empty;

                _logger?.LogInformation(
                    "Jellyfin Badges: TransformIndexHtml received {Length} characters, contents property found: {Found}",
                    contents.Length,
                    contentsProperty is not null);

                if (string.IsNullOrEmpty(contents))
                {
                    _logger?.LogWarning("Jellyfin Badges: TransformIndexHtml received empty/null contents, returning as-is");
                    return contents;
                }

                if (contents.Contains(InjectedScriptTag, StringComparison.Ordinal))
                {
                    _logger?.LogInformation("Jellyfin Badges: script tag already present in contents, skipping");
                    return contents;
                }

                const string HeadCloseTag = "</head>";
                var headIndex = contents.IndexOf(HeadCloseTag, StringComparison.OrdinalIgnoreCase);

                if (headIndex < 0)
                {
                    _logger?.LogWarning("Jellyfin Badges: TransformIndexHtml could not find </head> in contents");
                    return contents;
                }

                var result = contents.Insert(headIndex, InjectedScriptTag + "\n");
                _logger?.LogInformation("Jellyfin Badges: TransformIndexHtml successfully inserted script tag");
                return result;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Jellyfin Badges: TransformIndexHtml threw an exception");
                return payload?.GetType().GetProperty("contents")?.GetValue(payload)?.ToString() ?? string.Empty;
            }
        }
    }
}
