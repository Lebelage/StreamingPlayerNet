using LibVLCSharp.Shared;
using StreamingPlayer.Services.Interfaces;
using System.IO;
using System.Windows.Media;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;


namespace StreamingPlayer.Services
{
    class PlayerService : IPlayerService
    {
        public event EventHandler<float> PositionChanged;
        public event EventHandler<bool> MediaPlayerPlayingStateChanged;

        private LibVLC _LibVlc;
        private MediaPlayer _MediaPlayer;
        private Media _CurrentMedia;

        private bool _IsPlaying;
        public bool IsPlaying 
        { 
            get => _IsPlaying;
            private set => MediaPlayerPlayingStateChanged?.Invoke(this, value); 
        }
        public long _CurrentTime { get; private set; }
        public long _TotalTime { get; private set; }
        public float _Position { get; private set; }

        private int _IsSeekable;
        public bool IsSeekable 
        {
            get => _IsSeekable != 0;
        }

        public bool _IsSeeking { get; private set; }

        public void Initialize(LibVLC libVlc, MediaPlayer mediaPlayer)
        {
            _LibVlc = libVlc;
            _MediaPlayer = mediaPlayer;

            InitEvents();
        }

        public void StartSeeking()
        {
            _IsSeeking = true;
        }

        public void EndSeeking(float finalPosition)
        {
            _IsSeeking = false;
            if (IsSeekable)
            {
                _MediaPlayer.Position = finalPosition;
            }
        }

        public void Play(Stream stream)
        {           
            try
            {
                if (_LibVlc is null || stream is null)
                    throw new ArgumentNullException();

                var media = new Media(_LibVlc, new StreamMediaInput(stream));
                _MediaPlayer.Play(media);

                _MediaPlayer.Position = _Position;
            }
            catch (Exception ex)
            { }
        }

        public void Pause()
        {
            try
            {
                _MediaPlayer?.Pause();
            }
            catch (Exception ex)
            { }
        }

        public void SetNewPosition(float newPos)
        {
            try
            {
                _MediaPlayer.Position = newPos;
            }
            catch { }
        }

        private void InitEvents()
        {
            _MediaPlayer.Playing += (s, e) => IsPlaying = true;
            _MediaPlayer.Paused += (s, e) => IsPlaying = false;
            _MediaPlayer.Stopped += (s, e) => IsPlaying = false;

            _MediaPlayer.TimeChanged += (s, e) => _CurrentTime = e.Time;
            _MediaPlayer.LengthChanged += (s, e) => _TotalTime = e.Length;
            _MediaPlayer.PositionChanged += (s, e) =>
            {
                if (!_IsSeeking)
                {
                    _Position = e.Position;
                    PositionChanged?.Invoke(this, e.Position);
                }
            };          
            _MediaPlayer.SeekableChanged += (s, e) => _IsSeekable = e.Seekable;
        }


        private string FormatTime(long milliseconds)
        {
            if (milliseconds < 0) return "00:00:00";
            TimeSpan time = TimeSpan.FromMilliseconds(milliseconds);
            return time.ToString(@"hh\:mm\:ss");
        }


        public PlayerService(){}
    }
}
