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
    /// Runs once at server startup. Patches the served index.html to add a
    /// script tag pointing at this plugin's badges.js endpoint, so no
    /// manual JS Injector setup is required.
    ///
    /// This is the modern replacement for the removed IServerEntryPoint
    /// interface (removed in Jellyfin 10.9) - startup logic now runs via
    /// IHostedService, registered through IPluginServiceRegistrator.
    /// </summary>
    public class BadgesStartupService : IHostedService
    {
        private const string InjectedScriptTag =
            "<script defer src=\"Badges/script\"></script>";

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
            try
            {
                InjectScriptTag();
            }
            catch (UnauthorizedAccessException ex)
            {
                // Common in Docker deployments where the web root is owned
                // by a different user than the Jellyfin process. See README
                // for the volume-mount workaround.
                _logger.LogError(
                    ex,
                    "Jellyfin Badges: permission denied writing to index.html. " +
                    "If running in Docker, see the README for a volume-mount workaround.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Jellyfin Badges: failed to patch index.html");
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
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

            _logger.LogInformation("Jellyfin Badges: injected script tag into index.html");
        }
    }
}
