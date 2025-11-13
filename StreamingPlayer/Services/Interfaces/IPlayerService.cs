using LibVLCSharp.Shared;
using System.IO;

namespace StreamingPlayer.Services.Interfaces
{
    interface IPlayerService
    {
        public event EventHandler<float> PositionChanged;
        public event EventHandler<bool> MediaPlayerPlayingStateChanged;
        void Initialize(LibVLC libVlc, MediaPlayer mediaPlayer);
        void Play(Stream stream);
        void Pause();

        void SetNewPosition(float newPos);


    }
}
