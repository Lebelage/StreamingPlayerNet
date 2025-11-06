using LibVLCSharp.Shared;
using StreamingPlayer.Infrastructure.Commands;
using StreamingPlayer.ViewModels.Base;
using System.Windows.Input;


namespace StreamingPlayer.ViewModels
{
    internal class VlcPlayerControlViewModel : DisposableViewModel
    {
        #region Fields
        private MediaPlayer _Player;
        public MediaPlayer? Player { get => _Player; set => Set(ref _Player, value); }

        private LibVLC _VLC;
        #endregion

        #region Commands

        #region (Command) : Play video command
        public ICommand PlayVideoCommand { get; }
        private bool CanPlayVideoCommandExecute(object p) => true;
        private void OnPlayVideoCommandExecuted(object p) 
        {
            var media = new Media(_VLC, "C:\\ProgramData\\The.Gorge.2025.2160p.ATVP.WEB-DL.DDP5.1.Atmos.DV.HDR-DVT.mkv");
            Player?.Play(media);
        }
        #endregion

        #endregion

        #region Methods
        #endregion

        public VlcPlayerControlViewModel() 
        {
            PlayVideoCommand = new LambdaCommand(OnPlayVideoCommandExecuted, CanPlayVideoCommandExecute);

            Player = App.VlcPlayerCore._MediaPlayer;
            _VLC = App.VlcPlayerCore._LibVLC;

        }
    }
}
