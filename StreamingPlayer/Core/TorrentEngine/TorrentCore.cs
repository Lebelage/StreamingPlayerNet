using MonoTorrent;
using MonoTorrent.Client;
using System.IO;
using System.Reflection;

namespace StreamingPlayer.Core.TorrentEngine
{
    public class TorrentCore : IDisposable
    {
        #region Events
        public event EventHandler<TorrentStateChangedEventArgs?>? TorrentStateChanged;
        public event EventHandler<float?>? DownloadSpeedChanged;
        #endregion

        #region Fields
        private readonly ClientEngine _Engine;

        private TorrentManager _Manager;
        private string _DownloadPath;


        private bool _disposed = false;

        private Task _InfoCollectionTask;
        private Task _UpdatePriorityTask;
        private CancellationTokenSource _CancellationTokenSource;

        public Stream _Stream { get; private set; }
        #endregion

        #region Methods
        public async Task LoadTorrent(string torrentFilePath, string downloadDirectory)
        {
            try
            {
                _Stream?.Dispose();

                _DownloadPath = torrentFilePath;

                var torrent = await Torrent.LoadAsync(torrentFilePath);

                _Manager = await _Engine.AddStreamingAsync(torrent, downloadDirectory);

                var largestFile = _Manager.Files.OrderByDescending(t => t.Length).First();

                _Manager.TorrentStateChanged += (s, e) => TorrentStateChanged?.Invoke(this, e);
                
                await _Manager.StartAsync();

                var stream = await _Manager.StreamProvider.CreateStreamAsync(largestFile,true);

                _Stream = new BufferedStream(stream);
               
            }
            catch (Exception ex)
            {

            }
        }



        private void InfoCollection(CancellationToken token, uint timeout = 2000)
        {
            Task.Run(() =>
            {
                float lastState = 0;

                while (!token.IsCancellationRequested)
                {
                    float newState = _Engine.TotalDownloadRate;

                    if (newState != lastState)
                    {
                        lastState = newState;

                        DownloadSpeedChanged?.Invoke(this, newState);
                    }
                    Thread.Sleep((int)timeout);
                }
            });

        }
        #endregion

        #region Handlers
        //private void OnPieceHashed(object? sender, PieceHashedEventArgs e)
        //{
        //    if (e.HashPassed)
        //    {
        //        int firstMissingPiece = FindFirstMissingPiece();
        //        if (firstMissingPiece >= 0)
        //        {
        //            UpdatePiecePriorities(firstMissingPiece);
        //        }
        //    }
        //}

        //private void UpdatePiecePriorities(int startPiece)
        //{
        //    for (int i = 0; i < _Manager.Torrent.PieceCount; i++)
        //    {
        //        //_Manager.Se
        //    }
        //}

        #endregion


        public TorrentCore()
        {
            var a = new EngineSettings();
            
            _Engine = new ClientEngine(new EngineSettings());

            _CancellationTokenSource = new CancellationTokenSource();
            _InfoCollectionTask = Task.Run(() => InfoCollection(_CancellationTokenSource.Token), _CancellationTokenSource.Token);
        }

        #region Disposing
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _InfoCollectionTask?.Wait();
                _InfoCollectionTask?.Dispose();
                _CancellationTokenSource?.Dispose();
                _Engine?.Dispose();

            }

            _disposed = true;
        }
        #endregion
    }
}
