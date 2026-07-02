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

        /// <summary>
        /// Gets or sets the base URL where badge logo images are hosted.
        /// Individual logo filenames are appended to this.
        /// </summary>
        public string LogoBaseUrl { get; set; }

        public bool ShowRatingBadge { get; set; }

        public bool ShowResolutionBadge { get; set; }

        public bool ShowCodecBadge { get; set; }

        public bool ShowHdrBadge { get; set; }

        public bool ShowAudioBadge { get; set; }

        public bool ShowSourceBadge { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether HDR10/HDR10+/HDR badges
        /// should be hidden when Dolby Vision is present, since DV is
        /// already an HDR variant.
        /// </summary>
        public bool SuppressHdrWithDolbyVision { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the channel layout pill
        /// (e.g. "7.1") should be hidden when Atmos or DTS:X is shown,
        /// since both formats are object-based and imply the channel count.
        /// </summary>
        public bool SuppressChannelLayoutWithAtmosOrDtsX { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Atmos badge should
        /// render before the TrueHD badge (Atmos implies TrueHD 7.1).
        /// </summary>
        public bool AtmosBeforeTrueHd { get; set; }

        /// <summary>
        /// Gets or sets how often (ms) the client script polls for route
        /// changes to detect a new detail page.
        /// </summary>
        public int PollIntervalMs { get; set; }

        public bool Debug { get; set; }
    }
}
