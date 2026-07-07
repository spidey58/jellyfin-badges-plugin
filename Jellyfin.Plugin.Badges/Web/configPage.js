(function () {
    'use strict';

    var BadgesConfig = {
        pluginUniqueId: '2c0f75c3-6aad-4003-95e3-0fd418279ac3'
    };

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
                document.querySelector('#Res4kUrl').value = config.Res4kUrl || '';
                document.querySelector('#DolbyVisionUrl').value = config.DolbyVisionUrl || '';
                document.querySelector('#Hdr10Url').value = config.Hdr10Url || '';
                document.querySelector('#Hdr10PlusUrl').value = config.Hdr10PlusUrl || '';
                document.querySelector('#HdrUrl').value = config.HdrUrl || '';
                document.querySelector('#TrueHdUrl').value = config.TrueHdUrl || '';
                document.querySelector('#AtmosUrl').value = config.AtmosUrl || '';
                document.querySelector('#DtsUrl').value = config.DtsUrl || '';
                document.querySelector('#DtsXUrl').value = config.DtsXUrl || '';
                document.querySelector('#DolbyDigitalUrl').value = config.DolbyDigitalUrl || '';
                document.querySelector('#DolbyDigitalPlusUrl').value = config.DolbyDigitalPlusUrl || '';
                document.querySelector('#BlurayUhdUrl').value = config.BlurayUhdUrl || '';
                document.querySelector('#BlurayUrl').value = config.BlurayUrl || '';
                document.querySelector('#ImdbUrl').value = config.ImdbUrl || '';
                document.querySelector('#ShowRatingBadge').checked = config.ShowRatingBadge;
                document.querySelector('#ShowResolutionBadge').checked = config.ShowResolutionBadge;
                document.querySelector('#ShowCodecBadge').checked = config.ShowCodecBadge;
                document.querySelector('#ShowHdrBadge').checked = config.ShowHdrBadge;
                document.querySelector('#ShowAudioBadge').checked = config.ShowAudioBadge;
                document.querySelector('#ShowSourceBadge').checked = config.ShowSourceBadge;
                document.querySelector('#SuppressHdrWithDolbyVision').checked = config.SuppressHdrWithDolbyVision;
                document.querySelector('#SuppressChannelLayoutWithAtmosOrDtsX').checked = config.SuppressChannelLayoutWithAtmosOrDtsX;
                document.querySelector('#AtmosBeforeTrueHd').checked = config.AtmosBeforeTrueHd;
                document.querySelector('#PollIntervalMs').value = config.PollIntervalMs;
                document.querySelector('#Debug').checked = config.Debug;
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
                config.Res4kUrl = document.querySelector('#Res4kUrl').value;
                config.DolbyVisionUrl = document.querySelector('#DolbyVisionUrl').value;
                config.Hdr10Url = document.querySelector('#Hdr10Url').value;
                config.Hdr10PlusUrl = document.querySelector('#Hdr10PlusUrl').value;
                config.HdrUrl = document.querySelector('#HdrUrl').value;
                config.TrueHdUrl = document.querySelector('#TrueHdUrl').value;
                config.AtmosUrl = document.querySelector('#AtmosUrl').value;
                config.DtsUrl = document.querySelector('#DtsUrl').value;
                config.DtsXUrl = document.querySelector('#DtsXUrl').value;
                config.DolbyDigitalUrl = document.querySelector('#DolbyDigitalUrl').value;
                config.DolbyDigitalPlusUrl = document.querySelector('#DolbyDigitalPlusUrl').value;
                config.BlurayUhdUrl = document.querySelector('#BlurayUhdUrl').value;
                config.BlurayUrl = document.querySelector('#BlurayUrl').value;
                config.ImdbUrl = document.querySelector('#ImdbUrl').value;
                config.ShowRatingBadge = document.querySelector('#ShowRatingBadge').checked;
                config.ShowResolutionBadge = document.querySelector('#ShowResolutionBadge').checked;
                config.ShowCodecBadge = document.querySelector('#ShowCodecBadge').checked;
                config.ShowHdrBadge = document.querySelector('#ShowHdrBadge').checked;
                config.ShowAudioBadge = document.querySelector('#ShowAudioBadge').checked;
                config.ShowSourceBadge = document.querySelector('#ShowSourceBadge').checked;
                config.SuppressHdrWithDolbyVision = document.querySelector('#SuppressHdrWithDolbyVision').checked;
                config.SuppressChannelLayoutWithAtmosOrDtsX = document.querySelector('#SuppressChannelLayoutWithAtmosOrDtsX').checked;
                config.AtmosBeforeTrueHd = document.querySelector('#AtmosBeforeTrueHd').checked;
                config.PollIntervalMs = parseInt(document.querySelector('#PollIntervalMs').value, 10) || 800;
                config.Debug = document.querySelector('#Debug').checked;

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

        log('config script initialized successfully (loaded via dynamic script injection)');
    }

    init();
})();
