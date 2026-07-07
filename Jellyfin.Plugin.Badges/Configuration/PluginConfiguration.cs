using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.Badges.Configuration
{
    /// <summary>
    /// Configuration for the Jellyfin Badges plugin, editable from
    /// Dashboard > Plugins > Jellyfin Badges.
    /// </summary>
    public class PluginConfiguration : BasePluginConfiguration
    {
        private const string DefaultBase =
            "https://raw.githubusercontent.com/spidey58/jellyfinbadges/refs/heads/main/";

        public PluginConfiguration()
        {
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

            Res4kUrl = DefaultBase + "4k-icon.png";
            DolbyVisionUrl = DefaultBase + "logos-white/Dolby_Vision-white.png";
            Hdr10Url = DefaultBase + "logos-white/hdr10-white.png";
            Hdr10PlusUrl = DefaultBase + "logos-white/hdr10plus-white.png";
            HdrUrl = DefaultBase + "logos-white/hdr-icon.png";
            TrueHdUrl = DefaultBase + "logos-white/dolby-truehd-white.png";
            AtmosUrl = DefaultBase + "logos-white/Dolby_Atmos_Logo-700x260-white.png";
            DtsUrl = DefaultBase + "logos-white/dts-hd-master-audio.png";
            DtsXUrl = DefaultBase + "logos-white/dts-x.png";
            DolbyDigitalUrl = DefaultBase + "logos-white/dolby-digital-white.png";
            DolbyDigitalPlusUrl = DefaultBase + "logos-white/dolby-digital-plus-white.png";
            BlurayUhdUrl = DefaultBase + "logos-white/ultra-hd-blu-ray-seeklogo-white.png";
            BlurayUrl = DefaultBase + "logos-white/blu-ray-disc-seeklogo-white.png";
            ImdbUrl = DefaultBase + "logos-ratings/imdb.png";
        }

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

        // Individual, independently-editable logo URLs. Each defaults to
        // the spidey58/jellyfinbadges repo but can be pointed anywhere.
        public string Res4kUrl { get; set; }

        public string DolbyVisionUrl { get; set; }

        public string Hdr10Url { get; set; }

        public string Hdr10PlusUrl { get; set; }

        public string HdrUrl { get; set; }

        public string TrueHdUrl { get; set; }

        public string AtmosUrl { get; set; }

        public string DtsUrl { get; set; }

        public string DtsXUrl { get; set; }

        public string DolbyDigitalUrl { get; set; }

        public string DolbyDigitalPlusUrl { get; set; }

        public string BlurayUhdUrl { get; set; }

        public string BlurayUrl { get; set; }

        public string ImdbUrl { get; set; }
    }
}
