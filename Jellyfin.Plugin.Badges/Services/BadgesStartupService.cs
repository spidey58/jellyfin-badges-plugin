using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Badges.Services
{
    /// <summary>
    /// Runs once at server startup. Ensures the badges client script gets
    /// loaded by the web client, preferring the non-destructive File
    /// Transformation plugin integration and falling back to a direct
    /// index.html file write if that plugin is not installed.
    /// </summary>
    public class BadgesStartupService : IHostedService
    {
        private const string InjectedScriptTag =
            "<script defer src=\"/Badges/script\"></script>";

        private readonly IApplicationPaths _applicationPaths;
        private readonly ILogger<BadgesStartupService> _logger;

        public BadgesStartupService(
            IApplicationPaths applicationPaths,
            ILogger<BadgesStartupService> logger)
        {
            _applicationPaths = applicationPaths;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var registeredWithFileTransformation = FileTransformationIntegration.TryRegister(_logger);

            if (!registeredWithFileTransformation)
            {
                TryDirectFileWrite();
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void TryDirectFileWrite()
        {
            try
            {
                InjectScriptTag();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(
                    ex,
                    "Jellyfin Badges: permission denied writing to index.html, and the File " +
                    "Transformation plugin is not installed. Install 'File Transformation' from " +
                    "the plugin catalog for permission-free injection - see the README.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Jellyfin Badges: failed to patch index.html");
            }
        }

        private void InjectScriptTag()
        {
            var indexPath = Path.Combine(_applicationPaths.WebPath, "index.html");

            if (!File.Exists(indexPath))
            {
                _logger.LogWarning("Jellyfin Badges: index.html not found at {Path}", indexPath);
                return;
            }

            var contents = File.ReadAllText(indexPath);

            if (contents.Contains(InjectedScriptTag, StringComparison.Ordinal))
            {
                _logger.LogInformation("Jellyfin Badges: script tag already present, skipping injection");
                return;
            }

            const string HeadCloseTag = "</head>";
            var headIndex = contents.IndexOf(HeadCloseTag, StringComparison.OrdinalIgnoreCase);

            if (headIndex < 0)
            {
                _logger.LogWarning("Jellyfin Badges: could not find </head> in index.html");
                return;
            }

            var patched = contents.Insert(headIndex, InjectedScriptTag + "\n");
            File.WriteAllText(indexPath, patched);

            _logger.LogInformation("Jellyfin Badges: injected script tag into index.html directly");
        }
    }
}
