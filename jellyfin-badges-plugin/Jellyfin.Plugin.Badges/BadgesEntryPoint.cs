using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Controller.Plugins;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Badges
{
    /// <summary>
    /// Runs once at server startup. Patches the served index.html to add a
    /// script tag pointing at this plugin's badges.js endpoint, the same
    /// technique used by Intro Skipper and similar community plugins,
    /// so no manual JS Injector setup is required.
    /// </summary>
    public class BadgesEntryPoint : IServerEntryPoint
    {
        private const string InjectedScriptTag =
            "<script defer src=\"Badges/script\"></script>";

        private readonly IApplicationPaths _applicationPaths;
        private readonly ILogger<BadgesEntryPoint> _logger;

        public BadgesEntryPoint(
            IApplicationPaths applicationPaths,
            ILogger<BadgesEntryPoint> logger)
        {
            _applicationPaths = applicationPaths;
            _logger = logger;
        }

        public Task RunAsync()
        {
            try
            {
                InjectScriptTag();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Jellyfin Badges: failed to patch index.html");
            }

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

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
