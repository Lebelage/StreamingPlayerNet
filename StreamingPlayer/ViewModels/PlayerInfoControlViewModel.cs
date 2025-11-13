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

        #region CurrentState : string
        private string? _CurrentState;
        public string? CurrentState { get => _CurrentState; set => Set(ref _CurrentState, value); }
        #endregion

        #region CurrentSpeed : float
        private float? _CurrentSpeed;
        public float? CurrentSpeed { get => _CurrentSpeed; set => Set(ref _CurrentSpeed, value); }
        #endregion

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
        private void OnTorrentStateChanged(object? sender, MonoTorrent.Client.TorrentStateChangedEventArgs? e)
        {
            CurrentState = e?.NewState.ToString();
        }

        private void OnDownloadSpeedChanged(object? sender, float? e)
        {
            if (e is null)
                return;

            CurrentSpeed = e;
        }

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

            App.TorrentCore.TorrentStateChanged += OnTorrentStateChanged;
            App.TorrentCore.DownloadSpeedChanged += OnDownloadSpeedChanged;
        }     
        #endregion
    }
}
