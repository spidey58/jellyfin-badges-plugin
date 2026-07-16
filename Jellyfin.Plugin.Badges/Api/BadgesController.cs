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
                showResolutionBadge = config.ShowResolutionBadge,
                showCodecBadge = config.ShowCodecBadge,
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
                    bluray = config.BlurayUrl
                },
                enabled = new
                {
                    res4k = config.EnableRes4k,
                    dolbyVision = config.EnableDolbyVision,
                    hdr10 = config.EnableHdr10,
                    hdr10plus = config.EnableHdr10Plus,
                    hdr = config.EnableHdr,
                    truehd = config.EnableTrueHd,
                    atmos = config.EnableAtmos,
                    dts = config.EnableDts,
                    dtsx = config.EnableDtsX,
                    dolbyDigital = config.EnableDolbyDigital,
                    dolbyDigitalPlus = config.EnableDolbyDigitalPlus,
                    blurayUhd = config.EnableBlurayUhd,
                    bluray = config.EnableBluray
                }
            };

            return new JsonResult(payload);
        }
    }
}
