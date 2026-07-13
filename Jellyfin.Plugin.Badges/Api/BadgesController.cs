using System.Net.Mime;
using System.Reflection;
using Jellyfin.Plugin.Badges.Services;
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
        private readonly NfoRatingReader _nfoRatingReader;

        public BadgesController(NfoRatingReader nfoRatingReader)
        {
            _nfoRatingReader = nfoRatingReader;
        }

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
                    bluray = config.BlurayUrl,
                    imdb = config.ImdbUrl,
                    tmdb = config.TmdbUrl
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
                    bluray = config.EnableBluray,
                    imdb = config.EnableImdb,
                    tmdb = config.EnableTmdb
                }
            };

            return new JsonResult(payload);
        }

        /// <summary>
        /// Reads a named rating (e.g. tmdb) directly from the NFO file
        /// next to the given media file, since Jellyfin's own API only
        /// exposes a single collapsed CommunityRating value.
        /// </summary>
        [HttpGet("nfoRating")]
        [Produces(MediaTypeNames.Application.Json)]
        public ActionResult GetNfoRating([FromQuery] string path, [FromQuery] string source)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(source))
            {
                return BadRequest();
            }

            var names = source.Equals("tmdb", System.StringComparison.OrdinalIgnoreCase)
                ? new[] { "tmdb", "themoviedb" }
                : new[] { source };

            var rating = _nfoRatingReader.ReadRating(path, names);

            return new JsonResult(new { rating });
        }
    }
}
