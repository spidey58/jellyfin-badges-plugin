using Jellyfin.Plugin.Badges.Services;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace Jellyfin.Plugin.Badges
{
    /// <summary>
    /// Registers this plugin's services with Jellyfin's dependency
    /// injection container at server startup. This is the modern
    /// replacement for the removed IServerEntryPoint interface
    /// (removed in Jellyfin 10.9).
    /// </summary>
    public class PluginServiceRegistrator : IPluginServiceRegistrator
    {
        public void RegisterServices(IServiceCollection serviceCollection, IServerApplicationHost applicationHost)
        {
            serviceCollection.AddHostedService<BadgesStartupService>();
        }
    }
}

