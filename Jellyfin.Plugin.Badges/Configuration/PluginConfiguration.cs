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
            ShowResolutionBadge = true;
            ShowCodecBadge = true;
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
            TmdbUrl = DefaultBase + "logos-ratings/tmdb.png";

            EnableRes4k = true;
            EnableDolbyVision = true;
            EnableHdr10 = true;
            EnableHdr10Plus = true;
            EnableHdr = true;
            EnableTrueHd = true;
            EnableAtmos = true;
            EnableDts = true;
            EnableDtsX = true;
            EnableDolbyDigital = true;
            EnableDolbyDigitalPlus = true;
            EnableBlurayUhd = true;
            EnableBluray = true;
            EnableImdb = true;
            EnableTmdb = true;
        }

        public bool ShowResolutionBadge { get; set; }

        public bool ShowCodecBadge { get; set; }

        public bool SuppressHdrWithDolbyVision { get; set; }

        public bool SuppressChannelLayoutWithAtmosOrDtsX { get; set; }

        public bool AtmosBeforeTrueHd { get; set; }

        public int PollIntervalMs { get; set; }

        public bool Debug { get; set; }

        // Individual logo URLs and their matching enable toggles - kept
        // as pairs so the config page can show a checkbox right next to
        // each URL field.
        public string Res4kUrl { get; set; }

        public bool EnableRes4k { get; set; }

        public string DolbyVisionUrl { get; set; }

        public bool EnableDolbyVision { get; set; }

        public string Hdr10Url { get; set; }

        public bool EnableHdr10 { get; set; }

        public string Hdr10PlusUrl { get; set; }

        public bool EnableHdr10Plus { get; set; }

        public string HdrUrl { get; set; }

        public bool EnableHdr { get; set; }

        public string TrueHdUrl { get; set; }

        public bool EnableTrueHd { get; set; }

        public string AtmosUrl { get; set; }

        public bool EnableAtmos { get; set; }

        public string DtsUrl { get; set; }

        public bool EnableDts { get; set; }

        public string DtsXUrl { get; set; }

        public bool EnableDtsX { get; set; }

        public string DolbyDigitalUrl { get; set; }

        public bool EnableDolbyDigital { get; set; }

        public string DolbyDigitalPlusUrl { get; set; }

        public bool EnableDolbyDigitalPlus { get; set; }

        public string BlurayUhdUrl { get; set; }

        public bool EnableBlurayUhd { get; set; }

        public string BlurayUrl { get; set; }

        public bool EnableBluray { get; set; }

        public string ImdbUrl { get; set; }

        public bool EnableImdb { get; set; }

        public string TmdbUrl { get; set; }

        public bool EnableTmdb { get; set; }
    }
}
