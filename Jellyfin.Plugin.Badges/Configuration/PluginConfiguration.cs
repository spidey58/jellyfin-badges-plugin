using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.Badges.Configuration
{
    /// <summary>
    /// Configuration for the Jellyfin Badges plugin, editable from
    /// Dashboard > Plugins > Jellyfin Badges.
    /// </summary>
    public class PluginConfiguration : BasePluginConfiguration
    {
        public PluginConfiguration()
        {
            LogoBaseUrl = "https://raw.githubusercontent.com/spidey58/jellyfinbadges/refs/heads/main/";
            ShowRatingBadge = true;
            ShowResolutionBadge = true;
            ShowCodecBadge = true;
            ShowHdrBadge = true;
            ShowAudioBadge = true;
            ShowSourceBadge = true;
            SuppressHdrWithDolbyVision = true;
            SuppressChannelLayoutWithAtmosOrDtsX = true;
            AtmosBeforeTrueHd = true;
            PollIntervalMs = 800;
            Debug = false;
        }

        public string LogoBaseUrl { get; set; }

        public bool ShowRatingBadge { get; set; }

        public bool ShowResolutionBadge { get; set; }

        public bool ShowCodecBadge { get; set; }

        public bool ShowHdrBadge { get; set; }

        public bool ShowAudioBadge { get; set; }

        public bool ShowSourceBadge { get; set; }

        public bool SuppressHdrWithDolbyVision { get; set; }

        public bool SuppressChannelLayoutWithAtmosOrDtsX { get; set; }

        public bool AtmosBeforeTrueHd { get; set; }

        public int PollIntervalMs { get; set; }

        public bool Debug { get; set; }
    }
}
