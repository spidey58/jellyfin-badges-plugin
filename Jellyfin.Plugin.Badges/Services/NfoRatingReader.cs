using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Badges.Services
{
    /// <summary>
    /// Locates and parses the NFO file for a given media file path to
    /// extract named ratings (e.g. tmdb) that Jellyfin's own API collapses
    /// into a single CommunityRating value.
    /// </summary>
    public class NfoRatingReader
    {
        private readonly ILogger<NfoRatingReader> _logger;

        public NfoRatingReader(ILogger<NfoRatingReader> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Attempts to read a specific named rating (e.g. "tmdb", "imdb")
        /// from the NFO file matching the given media file path.
        /// </summary>
        /// <param name="mediaFilePath">Full path to the video file.</param>
        /// <param name="ratingName">Rating source name to look for, matched case-insensitively (e.g. "tmdb", "themoviedb").</param>
        /// <returns>The rating value if found, otherwise null.</returns>
        public double? ReadRating(string mediaFilePath, params string[] ratingNames)
        {
            try
            {
                var nfoPath = FindNfoPath(mediaFilePath);
                if (nfoPath is null)
                {
                    _logger.LogDebug("Jellyfin Badges: no NFO file found for {Path}", mediaFilePath);
                    return null;
                }

                var doc = XDocument.Load(nfoPath);

                // Standard Kodi/tMM NFO structure:
                // <movie>...<ratings><rating name="tmdb" max="10" default="false">
                //   <value>7.5</value><votes>1234</votes></rating></ratings>...</movie>
                var ratingElements = doc.Descendants("rating");

                foreach (var ratingName in ratingNames)
                {
                    var match = ratingElements.FirstOrDefault(r =>
                        string.Equals((string?)r.Attribute("name"), ratingName, StringComparison.OrdinalIgnoreCase));

                    if (match is not null)
                    {
                        var valueElement = match.Element("value");
                        if (valueElement is not null
                            && double.TryParse(valueElement.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var value))
                        {
                            _logger.LogDebug("Jellyfin Badges: found rating '{Name}' = {Value} in {Path}", ratingName, value, nfoPath);
                            return value;
                        }
                    }
                }

                _logger.LogDebug("Jellyfin Badges: NFO found at {Path} but no matching rating names {Names}", nfoPath, string.Join(",", ratingNames));
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Jellyfin Badges: failed to read NFO rating for {Path}", mediaFilePath);
                return null;
            }
        }

        /// <summary>
        /// Finds the NFO file matching a media file, trying the standard
        /// tMM/Kodi naming conventions in order.
        /// </summary>
        private string? FindNfoPath(string mediaFilePath)
        {
            if (string.IsNullOrEmpty(mediaFilePath))
            {
                return null;
            }

            var directory = Path.GetDirectoryName(mediaFilePath);
            var baseName = Path.GetFileNameWithoutExtension(mediaFilePath);

            if (string.IsNullOrEmpty(directory))
            {
                return null;
            }

            // 1. Same base name as the video file: "Movie Name.nfo"
            var sameNameNfo = Path.Combine(directory, baseName + ".nfo");
            if (File.Exists(sameNameNfo))
            {
                return sameNameNfo;
            }

            // 2. Generic "movie.nfo" in the same folder (common tMM convention)
            var genericNfo = Path.Combine(directory, "movie.nfo");
            if (File.Exists(genericNfo))
            {
                return genericNfo;
            }

            return null;
        }
    }
}
