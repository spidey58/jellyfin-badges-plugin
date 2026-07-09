(function () {
    'use strict';

    var BadgesConfig = {
        pluginUniqueId: '2c0f75c3-6aad-4003-95e3-0fd418279ac3'
    };

    var FIELDS = [
        'ImdbUrl', 'TmdbUrl', 'Res4kUrl', 'DolbyVisionUrl', 'Hdr10Url', 'Hdr10PlusUrl',
        'HdrUrl', 'AtmosUrl', 'TrueHdUrl', 'DtsXUrl', 'DtsUrl', 'DolbyDigitalUrl',
        'DolbyDigitalPlusUrl', 'BlurayUhdUrl', 'BlurayUrl'
    ];

    var CHECKBOXES = [
        'EnableImdb', 'EnableTmdb', 'EnableRes4k', 'EnableDolbyVision', 'EnableHdr10',
        'EnableHdr10Plus', 'EnableHdr', 'EnableAtmos', 'EnableTrueHd', 'EnableDtsX',
        'EnableDts', 'EnableDolbyDigital', 'EnableDolbyDigitalPlus', 'EnableBlurayUhd',
        'EnableBluray', 'ShowResolutionBadge', 'ShowCodecBadge',
        'SuppressHdrWithDolbyVision', 'SuppressChannelLayoutWithAtmosOrDtsX',
        'AtmosBeforeTrueHd', 'Debug'
    ];

    function log() {
        var args = Array.prototype.slice.call(arguments);
        args.unshift('[Jellyfin Badges]');
        console.log.apply(console, args);
    }

    function whenReady(callback, attempt) {
        attempt = attempt || 0;
        var ready = typeof window.ApiClient !== 'undefined'
            && typeof window.Dashboard !== 'undefined'
            && typeof window.Dashboard.showLoadingMsg === 'function';

        if (ready) {
            callback();
            return;
        }

        if (attempt > 40) {
            console.error('[Jellyfin Badges] ApiClient/Dashboard never became ready, giving up');
            return;
        }

        setTimeout(function () { whenReady(callback, attempt + 1); }, 150);
    }

    function loadConfig() {
        whenReady(function () {
            Dashboard.showLoadingMsg();
            ApiClient.getPluginConfiguration(BadgesConfig.pluginUniqueId).then(function (config) {
                for (var i = 0; i < FIELDS.length; i++) {
                    var el = document.querySelector('#' + FIELDS[i]);
                    if (el) el.value = config[FIELDS[i]] || '';
                }
                for (var j = 0; j < CHECKBOXES.length; j++) {
                    var cb = document.querySelector('#' + CHECKBOXES[j]);
                    if (cb) cb.checked = !!config[CHECKBOXES[j]];
                }
                document.querySelector('#PollIntervalMs').value = config.PollIntervalMs;
                log('configuration loaded into form', config);
                Dashboard.hideLoadingMsg();
            }).catch(function (err) {
                console.error('[Jellyfin Badges] failed to load configuration', err);
                Dashboard.hideLoadingMsg();
            });
        });
    }

    function saveConfig(e) {
        e.preventDefault();
        log('submit intercepted, saving configuration');

        whenReady(function () {
            Dashboard.showLoadingMsg();
            ApiClient.getPluginConfiguration(BadgesConfig.pluginUniqueId).then(function (config) {
                for (var i = 0; i < FIELDS.length; i++) {
                    var el = document.querySelector('#' + FIELDS[i]);
                    if (el) config[FIELDS[i]] = el.value;
                }
                for (var j = 0; j < CHECKBOXES.length; j++) {
                    var cb = document.querySelector('#' + CHECKBOXES[j]);
                    if (cb) config[CHECKBOXES[j]] = cb.checked;
                }
                config.PollIntervalMs = parseInt(document.querySelector('#PollIntervalMs').value, 10) || 800;

                ApiClient.updatePluginConfiguration(BadgesConfig.pluginUniqueId, config).then(function (result) {
                    log('configuration saved successfully', result);
                    Dashboard.processPluginConfigurationUpdateResult(result);
                }).catch(function (err) {
                    console.error('[Jellyfin Badges] updatePluginConfiguration failed', err);
                    Dashboard.hideLoadingMsg();
                });
            }).catch(function (err) {
                console.error('[Jellyfin Badges] getPluginConfiguration failed during save', err);
                Dashboard.hideLoadingMsg();
            });
        });

        return false;
    }

    function init() {
        var page = document.querySelector('#BadgesConfigPage');
        var form = document.querySelector('#BadgesConfigForm');

        if (!page || !form) {
            console.error('[Jellyfin Badges] page or form not found in DOM');
            return;
        }

        page.addEventListener('pageshow', loadConfig);
        loadConfig();

        form.addEventListener('submit', saveConfig);

        log('config script initialized successfully');
    }

    init();
})();
