using System;
using System.Collections.Generic;
using Jellyfin.Plugin.Badges.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace Jellyfin.Plugin.Badges
{
    /// <summary>
    /// Jellyfin Badges plugin entry point.
    /// Serves a client-side script that renders format and rating badges
    /// (Dolby Vision, HDR10, Atmos, TrueHD, IMDb, source type, etc.) on the
    /// movie/show detail page, sourced from Jellyfin's own MediaStreams and
    /// rating fields.
    /// </summary>
    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
            : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
        }

        public static Plugin? Instance { get; private set; }

        public override string Name => "Jellyfin Badges";

        public override Guid Id => Guid.Parse("2c0f75c3-6aad-4003-95e3-0fd418279ac3");

        public override string Description =>
            "Displays HDR/Dolby Vision, audio format, source, and rating badges on the detail page.";

        public IEnumerable<PluginPageInfo> GetPages()
        {
            return new[]
            {
                new PluginPageInfo
                {
                    Name = "badges",
                    EmbeddedResourcePath = string.Format(
                        System.Globalization.CultureInfo.InvariantCulture,
                        "{0}.Configuration.configPage.html",
                        GetType().Namespace)
                }
            };
        }
    }
}
