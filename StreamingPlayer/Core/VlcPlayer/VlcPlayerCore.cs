using LibVLCSharp.Shared;

namespace StreamingPlayer.Core.VlcPlayer
{
    public class VlcPlayerCore : IDisposable
    {
        #region Fields
        public LibVLC _LibVLC { get; private set; }
        public MediaPlayer _MediaPlayer { get; private set; }
        #endregion

        #region Methods
        private void Initialize() 
        {
            _LibVLC = new LibVLC();
            _MediaPlayer = new MediaPlayer(_LibVLC);
        }
        #endregion
        public VlcPlayerCore() 
        {
            Initialize();
        }

        public void Dispose()
        {
            
        }
    }
}
