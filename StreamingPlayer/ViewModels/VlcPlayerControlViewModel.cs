using LibVLCSharp.Shared;
using LibVLCSharp.WPF;
using Microsoft.Extensions.DependencyInjection;
using StreamingPlayer.Core.TorrentEngine;
using StreamingPlayer.Infrastructure.Commands;
using StreamingPlayer.Services;
using StreamingPlayer.Services.Interfaces;
using StreamingPlayer.ViewModels.Base;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;



namespace StreamingPlayer.ViewModels
{
    internal class VlcPlayerControlViewModel : DisposableViewModel
    {
        #region Fields

        private MediaPlayer _Player;
        public MediaPlayer? Player { get => _Player; set => Set(ref _Player, value); }

        private float _CurrentPosition;
        public float CurrentPosition
        {
            get { return _CurrentPosition; }
            set 
            {               
                Set(ref _CurrentPosition, value);
            }
        }

        private bool _IsPalying;
        public bool IsPalying
        {
            get { return _IsPalying; }
            set
            {
                PlayerButtonName = value ? "Pause" : "Play";
                Set(ref _IsPalying, value);
            }
        }

        private string _PlayerButtonName = "Play";
        public string PlayerButtonName
        {
            get { return _PlayerButtonName; }
            set
            {
                Set(ref _PlayerButtonName, value);
            }
        }

        private IPlayerService playerService;
        #endregion

        #region Commands

        #region (Command) : SwitchPlayerStateCommand
        public ICommand SwitchPlayerStateCommand { get; }
        private bool CanSwitchPlayerStateCommandExecute(object p) => true;
        private async void OnSwitchPlayerStateCommandExecuted(object p)
        {
            if (!IsPalying) 
            {
                Play();               
            }
            else 
            {
                Pause();
            }
        }
        #endregion     

        #endregion

        #region Methods
        private void Play() 
        {
            try
            {
                var stream = App.TorrentCore._Stream;

                if (stream is null)
                    return;

                playerService?.Play(stream);
            }
            catch
            {

            }
        }
        private void Pause() 
        {
            try
            {
                Player?.Pause();
            }
            catch
            { }
        }
        
        private void ChangePosition() 
        {
            try
            {
                playerService.SetNewPosition(CurrentPosition);
            }
            catch
            {}
        }
        #endregion

        #region Handlers
        private void OnPositionChanged(object? sender, float e)
        {
            //CurrentPosition = e;
        }
        private void OnMediaPlayerPlayingStateChanged(object? sender, bool e)
        {
            IsPalying = e;
        }
        #endregion

        public VlcPlayerControlViewModel()
        {
            SwitchPlayerStateCommand = new LambdaCommand(OnSwitchPlayerStateCommandExecuted, CanSwitchPlayerStateCommandExecute);

            Player = App.VlcPlayerCore._MediaPlayer;

            playerService = App.Services.GetRequiredService<IPlayerService>();
            playerService.PositionChanged += OnPositionChanged;
            playerService.MediaPlayerPlayingStateChanged += OnMediaPlayerPlayingStateChanged;
            playerService.Initialize(App.VlcPlayerCore._LibVlc, Player);


        }

        
    }
}
