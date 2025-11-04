using Microsoft.Extensions.DependencyInjection;
using StreamingPlayer.ViewModels.Base;

namespace StreamingPlayer.ViewModels
{
    internal class ViewModelLocator : ViewModel
    {
        public MainWindowViewModel MainWindowVM => App.Services.GetRequiredService<MainWindowViewModel>();
        public PlayerInfoControlViewModel PlayerInfoVM => App.Services.GetRequiredService<PlayerInfoControlViewModel>();
    }
}
