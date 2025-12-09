using Microsoft.Extensions.DependencyInjection;
using StreamingPlayer.Infrastructure.Commands;
using StreamingPlayer.Services.Interfaces;
using StreamingPlayer.ViewModels.Base;
using System.Windows.Input;

namespace StreamingPlayer.ViewModels
{
    internal class MainWindowViewModel : DisposableViewModel
    {
        #region Fields

        private IEventNotification eventNotification;
        private IFileDialog fileDialog;
        #endregion

        #region Commands
        public ICommand OpenFileDialogCommand { get; }
        private bool CanOpenFileDialogCommandExecute(object p) => true;
        private void OnOpenFileDialogCommandExecuted(object p) => SelectFile();

        #endregion

        #region Methods
        private async void SelectFile() 
        {
            await fileDialog.OpenFileAsync();
        }
        #endregion

        #region Handlers
        private void OnTorrentFileSelected(object? sender, string? e)
        {
            if (e is null)
                return;

            App.TorrentCore.LoadTorrent(e, "C:/ProgramData");
        }
        #endregion

        #region Constructor
        public MainWindowViewModel() 
        {
            eventNotification = App.Services.GetRequiredService<IEventNotification>();
            fileDialog = App.Services.GetRequiredService<IFileDialog>();

            OpenFileDialogCommand = new LambdaCommand(OnOpenFileDialogCommandExecuted, CanOpenFileDialogCommandExecute);

            eventNotification.TorrentFileSelected += OnTorrentFileSelected;
        }

        #endregion
    }
}
