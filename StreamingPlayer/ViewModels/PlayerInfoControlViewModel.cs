using Microsoft.Extensions.DependencyInjection;
using MonoTorrent.BEncoding;
using StreamingPlayer.Infrastructure.Commands;
using StreamingPlayer.Services.Interfaces;
using StreamingPlayer.ViewModels.Base;
using System.Windows.Input;

namespace StreamingPlayer.ViewModels
{
    internal class PlayerInfoControlViewModel : DisposableViewModel
    {
        #region Fields

        #region SelectedFilePath : string
        private string? _SelectedFileName;
        public string? SelectedFileName { get => _SelectedFileName; set => Set(ref _SelectedFileName, value); }
        #endregion

        private IEventNotification eventNotification;
        #endregion

        #region Commands
        #endregion

        #region Methods
        #endregion

        #region Handlers
        private void OnTorrentFileSelected(object? sender, string? e)
        {
            if (e is not null)
                SelectedFileName = e;
        }
        #endregion

        #region Constructor
        public PlayerInfoControlViewModel()
        {
            eventNotification = App.Services.GetRequiredService<IEventNotification>();

            eventNotification.TorrentFileSelected += OnTorrentFileSelected;
        }


        #endregion
    }
}
