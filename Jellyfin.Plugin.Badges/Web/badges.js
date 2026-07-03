/**
 * Jellyfin Badges - client script
 * -----------------------------------------------------------------------
 * Served by the Jellyfin Badges plugin at /Badges/script and auto-injected
 * into index.html by BadgesEntryPoint on server startup.
 *
 * Configuration (logo URLs, feature toggles) is fetched at runtime from
 * /Badges/config, which reflects whatever is set in Dashboard > Plugins >
 * Jellyfin Badges - no need to edit this file to change settings.
 * -----------------------------------------------------------------------
 */
(function () {
    'use strict';

    var CONFIG = null;
    var POLL_INTERVAL_FALLBACK = 800;
    var pollHandle = null;

    function log() {
        if (!CONFIG || !CONFIG.debug) return;
        var args = Array.prototype.slice.call(arguments);
        args.unshift('[JF-Badges]');
        console.log.apply(console, args);
    }

    var BADGE_STYLES = {
        resolution:      { bg: '#1c1c1f', color: '#d4d4d8', border: '#2e2e33' },
        codec:           { bg: '#1c1c1f', color: '#9a9aa0', border: '#2e2e33' },
        dolbyVision:     { bg: '#1a1424', color: '#c9a8ff', border: '#3d2a52' },
        hdr10plus:       { bg: '#1f1a14', color: '#f0d29a', border: '#3a2f1f' },
        hdr10:           { bg: '#1f1a14', color: '#e8c98a', border: '#3a2f1f' },
        hdr:             { bg: '#1f1a14', color: '#dcb878', border: '#3a2f1f' },
        truehd:          { bg: '#14191f', color: '#8ec9f5', border: '#1f3245' },
        atmos:           { bg: '#14191f', color: '#8ec9f5', border: '#1f3245' },
        dts:             { bg: '#14191f', color: '#7fb8d9', border: '#1f3245' },
        dtsx:            { bg: '#14191f', color: '#7fb8d9', border: '#1f3245' },
        dolbyDigital:    { bg: '#14191f', color: '#7fb8d9', border: '#1f3245' },
        dolbyDigitalPlus:{ bg: '#14191f', color: '#7fb8d9', border: '#1f3245' },
        audioGeneric:    { bg: '#14191f', color: '#7fb8d9', border: '#1f3245' },
        webdl:           { bg: '#0d1a2e', color: '#60a5fa', border: '#1e3a5f' },
        rating:          { bg: '#1f1a08', color: '#f0c93f', border: '#4a3a10' }
    };

    // Maps a logical badge key to its filename under logoBaseUrl.
    var LOGO_FILES = {
        res4k:            '4k-icon.png',
        dolbyVision:      'logos-white/Dolby_Vision-white.png',
        hdr10:            'logos-white/hdr10-white.png',
        hdr10plus:        'logos-white/hdr10plus-white.png',
        hdr:              'logos-white/hdr-icon.png',
        truehd:           'logos-white/dolby-truehd-white.png',
        atmos:            'logos-white/Dolby_Atmos_Logo-700x260-white.png',
        dts:              'logos-white/dts-hd-master-audio.png',
        dtsx:             'logos-white/dts-x.png',
        dolbyDigital:     'logos-white/dolby-digital-white.png',
        dolbyDigitalPlus: 'logos-white/dolby-digital-plus-white.png',
        blurayUhd:        'logos-white/ultra-hd-blu-ray-seeklogo-white.png',
        bluray:           'logos-white/blu-ray-disc-seeklogo-white.png',
        imdb:             'logos-ratings/imdb.png'
    };

    function logoUrl(key) {
        if (!CONFIG || !CONFIG.logoBaseUrl) return null;
        var file = LOGO_FILES[key];
        if (!file) return null;
        return CONFIG.logoBaseUrl.replace(/\/$/, '') + '/' + file;
    }

    function makeBadge(text, styleKey) {
        var url = logoUrl(styleKey);
        if (url) {
            var wrapper = document.createElement('span');
            wrapper.className = 'jf-badge jf-badge-logo jf-badge-' + styleKey;
            wrapper.style.display = 'inline-flex';
            wrapper.style.alignItems = 'center';
            wrapper.style.margin = '0 10px 0 0';
            wrapper.style.verticalAlign = 'middle';
            var img = document.createElement('img');
            img.src = url;
            img.alt = text;
            img.style.height = '20px';
            img.style.width = 'auto';
            img.style.objectFit = 'contain';
            img.style.display = 'block';
            wrapper.appendChild(img);
            return wrapper;
        }
        var style = BADGE_STYLES[styleKey] || BADGE_STYLES.codec;
        var span = document.createElement('span');
        span.className = 'jf-badge jf-badge-' + styleKey;
        span.textContent = text;
        span.style.display = 'inline-flex';
        span.style.alignItems = 'center';
        span.style.height = '26px';
        span.style.padding = '0 10px';
        span.style.margin = '0 6px 0 0';
        span.style.borderRadius = '5px';
        span.style.fontSize = '11px';
        span.style.fontWeight = '700';
        span.style.letterSpacing = '0.03em';
        span.style.background = style.bg;
        span.style.color = style.color;
        span.style.border = '1px solid ' + style.border;
        span.style.whiteSpace = 'nowrap';
        return span;
    }

    function makeRatingBadge(score) {
        var wrapper = document.createElement('span');
        wrapper.className = 'jf-badge jf-badge-rating';
        wrapper.style.display = 'inline-flex';
        wrapper.style.alignItems = 'center';
        wrapper.style.gap = '6px';
        wrapper.style.margin = '0 10px 0 0';
        wrapper.style.verticalAlign = 'middle';
        var url = logoUrl('imdb');
        if (url) {
            var img = document.createElement('img');
            img.src = url;
            img.alt = 'IMDb';
            img.style.height = '20px';
            img.style.width = 'auto';
            img.style.objectFit = 'contain';
            img.style.display = 'block';
            wrapper.appendChild(img);
        }
        var scoreSpan = document.createElement('span');
        scoreSpan.textContent = score.toFixed(1);
        scoreSpan.style.fontSize = '13px';
        scoreSpan.style.fontWeight = '700';
        scoreSpan.style.color = '#f0c93f';
        wrapper.appendChild(scoreSpan);
        return wrapper;
    }

    function getSourceBadge(source) {
        if (!source) return null;
        var path = (source.Path || source.Name || '').toUpperCase();
        var container = (source.Container || '').toUpperCase();
        var protocol = (source.Protocol || '').toUpperCase();

        var isRemux = path.indexOf('REMUX') !== -1 || path.indexOf('BDMV') !== -1 || path.indexOf('BACKUP') !== -1;
        var isBluray = path.indexOf('BLURAY') !== -1 || path.indexOf('BLU-RAY') !== -1 || container === 'BLURAY';
        var isUhd = path.indexOf('UHD') !== -1 || path.indexOf('2160P') !== -1 || (isRemux && path.indexOf('2160') !== -1);
        var isWebDl = path.indexOf('WEB-DL') !== -1 || path.indexOf('WEBDL') !== -1 || path.indexOf('WEB.DL') !== -1 || protocol === 'HTTP';

        if (isUhd && (isRemux || isBluray)) return { text: 'UHD BLURAY', styleKey: 'blurayUhd' };
        if (isRemux || isBluray) return { text: 'BLURAY', styleKey: 'bluray' };
        if (isWebDl) return { text: 'WEB-DL', styleKey: 'webdl' };
        return null;
    }

    function getVideoBadges(stream) {
        var badges = [];
        if (!stream) return badges;

        if (CONFIG.showResolutionBadge) {
            if (stream.Height >= 2000) badges.push({ text: '4K', styleKey: 'res4k' });
            else if (stream.Height >= 1000) badges.push({ text: '1080p', styleKey: 'resolution' });
            else if (stream.Height >= 700) badges.push({ text: '720p', styleKey: 'resolution' });
        }

        if (CONFIG.showCodecBadge && stream.Codec) {
            var c = stream.Codec.toUpperCase();
            if (c === 'HEVC' || c === 'H265') badges.push({ text: 'HEVC', styleKey: 'codec' });
            else if (c === 'H264' || c === 'AVC') badges.push({ text: 'H.264', styleKey: 'codec' });
            else if (c === 'AV1') badges.push({ text: 'AV1', styleKey: 'codec' });
        }

        if (CONFIG.showHdrBadge) {
            var vrt = (stream.VideoRangeType || '').toUpperCase();
            if (vrt.indexOf('DOVI') === 0) {
                badges.push({ text: 'DOLBY VISION', styleKey: 'dolbyVision' });
                if (!CONFIG.suppressHdrWithDolbyVision) {
                    if (vrt.indexOf('HDR10PLUS') !== -1) badges.push({ text: 'HDR10+', styleKey: 'hdr10plus' });
                    else if (vrt.indexOf('HDR10') !== -1) badges.push({ text: 'HDR10', styleKey: 'hdr10' });
                }
            } else if (vrt === 'HDR10PLUS') {
                badges.push({ text: 'HDR10+', styleKey: 'hdr10plus' });
            } else if (vrt === 'HDR10') {
                badges.push({ text: 'HDR10', styleKey: 'hdr10' });
            } else if (vrt === 'HDR') {
                badges.push({ text: 'HDR', styleKey: 'hdr' });
            }
        }

        return badges;
    }

    function getAudioBadges(stream) {
        var badges = [];
        if (!CONFIG.showAudioBadge || !stream || !stream.Codec) return badges;
        var c = stream.Codec.toUpperCase();
        var profile = (stream.Profile || '').toUpperCase();
        var hasAtmos = false;
        var hasDtsX = false;

        if (c === 'TRUEHD') {
            var isAtmos = profile.indexOf('ATMOS') !== -1;
            if (isAtmos && CONFIG.atmosBeforeTrueHd) {
                badges.push({ text: 'ATMOS', styleKey: 'atmos' });
                hasAtmos = true;
                badges.push({ text: 'TRUEHD', styleKey: 'truehd' });
            } else {
                badges.push({ text: 'TRUEHD', styleKey: 'truehd' });
                if (isAtmos) {
                    badges.push({ text: 'ATMOS', styleKey: 'atmos' });
                    hasAtmos = true;
                }
            }
        } else if (c === 'EAC3' && profile.indexOf('ATMOS') !== -1) {
            badges.push({ text: 'ATMOS', styleKey: 'atmos' });
            hasAtmos = true;
        } else if (c === 'DTS') {
            if (profile.indexOf('X') !== -1) {
                badges.push({ text: 'DTS:X', styleKey: 'dtsx' });
                hasDtsX = true;
            } else if (profile.indexOf('HD MA') !== -1 || profile.indexOf('MA') !== -1) {
                badges.push({ text: 'DTS-HD MA', styleKey: 'dts' });
            } else {
                badges.push({ text: 'DTS', styleKey: 'dts' });
            }
        } else if (c === 'AC3') {
            badges.push({ text: 'DOLBY DIGITAL', styleKey: 'dolbyDigital' });
        } else if (c === 'EAC3') {
            badges.push({ text: 'DOLBY DIGITAL+', styleKey: 'dolbyDigitalPlus' });
        } else if (c === 'FLAC') {
            badges.push({ text: 'FLAC', styleKey: 'audioGeneric' });
        }

        var suppressChannels = CONFIG.suppressChannelLayoutWithAtmosOrDtsX && (hasAtmos || hasDtsX);
        if (stream.ChannelLayout && !suppressChannels) {
            badges.push({ text: stream.ChannelLayout, styleKey: 'audioGeneric' });
        }

        return badges;
    }

    function fetchItem(itemId, callback) {
        var api = window.ApiClient;
        if (!api) { log('ApiClient not found'); callback(null); return; }
        api.getItem(api.getCurrentUserId(), itemId).then(function (item) {
            callback(item);
        }).catch(function (e) {
            log('fetchItem error:', e);
            callback(null);
        });
    }

    function getCurrentItemId() {
        var full = window.location.href;
        var match = full.match(/[?&]id=([a-zA-Z0-9]+)/);
        if (match) { return match[1]; }
        return null;
    }

    function injectBadges() {
        if (!CONFIG) return;
        var itemId = getCurrentItemId();
        if (!itemId) return;

        var allRows = document.querySelectorAll('.jf-badges-row');
        var existing = null;
        for (var r = 0; r < allRows.length; r++) {
            if (allRows[r].getAttribute('data-item-id') === itemId) { existing = allRows[r]; break; }
        }
        if (existing) return;

        var miscInfoRow = document.querySelector('.itemMiscInfo.itemMiscInfo-primary');
        if (!miscInfoRow) return;

        fetchItem(itemId, function (item) {
            if (!item) return;
            log('Got item:', item.Name, 'Rating:', item.CommunityRating);

            var stale = document.querySelectorAll('.jf-badges-row');
            for (var s = 0; s < stale.length; s++) { stale[s].parentNode.removeChild(stale[s]); }

            var row = document.createElement('div');
            row.className = 'jf-badges-row';
            row.setAttribute('data-item-id', itemId);
            row.style.display = 'contents';

            if (CONFIG.showRatingBadge && item.CommunityRating) {
                row.appendChild(makeRatingBadge(item.CommunityRating));
            }

            var source = (item.MediaSources && item.MediaSources[0]) || {};
            var streams = source.MediaStreams || item.MediaStreams || [];
            var video = null;
            var audio = null;
            var defaultAudio = null;

            for (var i = 0; i < streams.length; i++) {
                if (streams[i].Type === 'Video' && !video) video = streams[i];
                if (streams[i].Type === 'Audio' && streams[i].IsDefault && !defaultAudio) defaultAudio = streams[i];
                if (streams[i].Type === 'Audio' && !audio) audio = streams[i];
            }
            if (defaultAudio) audio = defaultAudio;

            var vBadges = getVideoBadges(video);
            for (var v = 0; v < vBadges.length; v++) {
                row.appendChild(makeBadge(vBadges[v].text, vBadges[v].styleKey));
            }

            var aBadges = getAudioBadges(audio);
            for (var a = 0; a < aBadges.length; a++) {
                row.appendChild(makeBadge(aBadges[a].text, aBadges[a].styleKey));
            }

            if (CONFIG.showSourceBadge) {
                var srcBadge = getSourceBadge(source);
                if (srcBadge) {
                    row.appendChild(makeBadge(srcBadge.text, srcBadge.styleKey));
                }
            }

            var starContainer = miscInfoRow.querySelector('.starRatingContainer');
            if (starContainer) {
                starContainer.style.display = 'none';
                starContainer.parentNode.insertBefore(row, starContainer.nextSibling);
            } else {
                miscInfoRow.appendChild(row);
            }
        });
    }

    function loadConfigAndStart() {
        var api = window.ApiClient;
        if (!api) {
            setTimeout(loadConfigAndStart, 500);
            return;
        }

        api.ajax({
            url: api.getUrl('Badges/config'),
            type: 'GET',
            dataType: 'json'
        }).then(function (cfg) {
            CONFIG = cfg;
            log('Config loaded, polling every', CONFIG.pollIntervalMs, 'ms');
            if (pollHandle) clearInterval(pollHandle);
            pollHandle = setInterval(injectBadges, CONFIG.pollIntervalMs || POLL_INTERVAL_FALLBACK);
        }).catch(function (e) {
            console.error('[JF-Badges] Failed to load config, retrying in 3s', e);
            setTimeout(loadConfigAndStart, 3000);
        });
    }

    loadConfigAndStart();
})();
