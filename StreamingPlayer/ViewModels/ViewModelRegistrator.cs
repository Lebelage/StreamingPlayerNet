using Microsoft.Extensions.DependencyInjection;

namespace StreamingPlayer.ViewModels
{
    internal static class ViewModelRegistrator
    {
        public static IServiceCollection AddViewModels(this IServiceCollection services) => services
            .AddSingleton<MainWindowViewModel>()
            .AddSingleton<PlayerInfoControlViewModel>()
            .AddSingleton<VlcPlayerControlViewModel>()
            ;
    }
}
