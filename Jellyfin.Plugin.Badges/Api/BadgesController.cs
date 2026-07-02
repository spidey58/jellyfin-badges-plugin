using System.Net.Mime;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jellyfin.Plugin.Badges.Api
{
    /// <summary>
    /// Serves the client-side badges script and its runtime configuration.
    /// The script itself is embedded in the plugin assembly; configuration
    /// values (logo URLs, feature toggles) come from the admin-editable
    /// plugin settings so the JS never needs to be edited by hand.
    /// </summary>
    [ApiController]
    [Route("Badges")]
    [AllowAnonymous]
    public class BadgesController : ControllerBase
    {
        /// <summary>
        /// Returns the badges.js client script.
        /// </summary>
        [HttpGet("script")]
        [Produces("application/javascript")]
        public ActionResult GetScript()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"{GetType().Namespace!.Replace(".Api", string.Empty)}.Web.badges.js";
            var stream = assembly.GetManifestResourceStream(resourceName);

            if (stream is null)
            {
                return NotFound();
            }

            return File(stream, "application/javascript");
        }

        /// <summary>
        /// Returns the current plugin configuration as JSON so the
        /// client script can read logo URLs and feature toggles without
        /// them being hardcoded.
        /// </summary>
        [HttpGet("config")]
        [Produces(MediaTypeNames.Application.Json)]
        public ActionResult GetConfig()
        {
            var config = Plugin.Instance?.Configuration;
            if (config is null)
            {
                return NotFound();
            }

            var payload = new
            {
                logoBaseUrl = config.LogoBaseUrl,
                showRatingBadge = config.ShowRatingBadge,
                showResolutionBadge = config.ShowResolutionBadge,
                showCodecBadge = config.ShowCodecBadge,
                showHdrBadge = config.ShowHdrBadge,
                showAudioBadge = config.ShowAudioBadge,
                showSourceBadge = config.ShowSourceBadge,
                suppressHdrWithDolbyVision = config.SuppressHdrWithDolbyVision,
                suppressChannelLayoutWithAtmosOrDtsX = config.SuppressChannelLayoutWithAtmosOrDtsX,
                atmosBeforeTrueHd = config.AtmosBeforeTrueHd,
                pollIntervalMs = config.PollIntervalMs,
                debug = config.Debug
            };

            return new JsonResult(payload);
        }
    }
}
