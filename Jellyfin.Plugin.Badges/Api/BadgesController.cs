using System.Net.Mime;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jellyfin.Plugin.Badges.Api
{
    /// <summary>
    /// Serves the client-side badges script and its runtime configuration.
    /// </summary>
    [ApiController]
    [Route("Badges")]
    [AllowAnonymous]
    public class BadgesController : ControllerBase
    {
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

        [HttpGet("configScript")]
        [Produces("application/javascript")]
        public ActionResult GetConfigScript()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"{GetType().Namespace!.Replace(".Api", string.Empty)}.Web.configPage.js";
            var stream = assembly.GetManifestResourceStream(resourceName);

            if (stream is null)
            {
                return NotFound();
            }

            return File(stream, "application/javascript");
        }

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
                debug = config.Debug,
                logos = new
                {
                    res4k = config.Res4kUrl,
                    dolbyVision = config.DolbyVisionUrl,
                    hdr10 = config.Hdr10Url,
                    hdr10plus = config.Hdr10PlusUrl,
                    hdr = config.HdrUrl,
                    truehd = config.TrueHdUrl,
                    atmos = config.AtmosUrl,
                    dts = config.DtsUrl,
                    dtsx = config.DtsXUrl,
                    dolbyDigital = config.DolbyDigitalUrl,
                    dolbyDigitalPlus = config.DolbyDigitalPlusUrl,
                    blurayUhd = config.BlurayUhdUrl,
                    bluray = config.BlurayUrl,
                    imdb = config.ImdbUrl
                }
            };

            return new JsonResult(payload);
        }
    }
}
