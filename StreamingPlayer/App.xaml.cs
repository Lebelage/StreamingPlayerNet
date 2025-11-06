using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StreamingPlayer.Core.TorrentEngine;
using StreamingPlayer.Core.VlcPlayer;
using StreamingPlayer.Services;
using StreamingPlayer.ViewModels;
using System.Windows;

namespace StreamingPlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static TorrentCore TorrentCore { get; private set; }
        public static VlcPlayerCore VlcPlayerCore { get; private set; }

        public static Window? FocusedWindow => Current.Windows.Cast<Window>().FirstOrDefault(window => window.IsFocused);
        public static Window? ActivedWindow => Current.Windows.Cast<Window>().FirstOrDefault(window => window.IsActive);

        private static IHost? __Host;
        public static IHost Host => __Host ??= Program.CreateHostBuilder(Environment.GetCommandLineArgs()).Build();
        
        public static IServiceProvider Services => Host.Services;

        public static void ConfigureServices(HostBuilderContext host, IServiceCollection services) => services
            .AddServices()
            .AddViewModels();

        protected override async void OnStartup(StartupEventArgs e)
        {
            TorrentCore = new TorrentCore();
            VlcPlayerCore = new VlcPlayerCore();

            IHost host = Host;
            base.OnStartup(e);

            await host.StartAsync();
        
        }

        /// <summary>
        /// Stops host
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnExit(ExitEventArgs e)
        {

            base.OnExit(e);

            using (Host) await Host.StopAsync();
        }
    }

}
