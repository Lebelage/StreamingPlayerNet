
using LibVLCSharp.Shared;

namespace StreamingPlayer.Core.VlcPalyer
{
    public class VLCPlayerCore : IDisposable
    {
        public MediaPlayer _MediaPlayer { get; private set; }
        public LibVLC _LibVlc { get; private set; }

        public VLCPlayerCore()
        {
            _LibVlc = new LibVLC();
            _MediaPlayer = new MediaPlayer(_LibVlc);
        }
        ~VLCPlayerCore() => Dispose(); 

        public void Dispose()
        {
            _LibVlc?.Dispose();
            _MediaPlayer?.Dispose();
        }
    }
}
