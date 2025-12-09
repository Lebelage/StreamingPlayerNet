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
        public Stream _Stream { get; private set; }

        private Task _InfoCollectionTask;
        private Task _UpdatePriorityTask;
        private CancellationTokenSource _CancellationTokenSource;


        private bool _disposed = false;
        #endregion

        #region Methods
        public async Task LoadTorrent(string torrentFilePath, string downloadDirectory)
        {
            try
            {
                _Stream?.Dispose();

                var torrent = await Torrent.LoadAsync(torrentFilePath);

                _Manager = await _Engine.AddStreamingAsync(torrent, downloadDirectory);

                var largestFile = _Manager.Files.OrderByDescending(t => t.Length).First();

                _Manager.TorrentStateChanged += (s, e) => TorrentStateChanged?.Invoke(this, e);

                await _Manager.StartAsync();

                var stream = await _Manager.StreamProvider.CreateStreamAsync(largestFile, true);

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
        #endregion


        public TorrentCore()
        {
            var a = new EngineSettings();

            _Engine = new ClientEngine(new EngineSettings());

            _CancellationTokenSource = new CancellationTokenSource();
            _InfoCollectionTask = Task.Run(() => InfoCollection(_CancellationTokenSource.Token), _CancellationTokenSource.Token);
        }

        ~TorrentCore()
        {
            Dispose(true);
        }

        #region Disposing
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void CleanUp()
        {
            if (_Manager.Torrent == null)
            {
                return;
            }

            string savePath = _Manager.SavePath;
            string rootName = _Manager.Torrent.Name;

            string fullDownloadPath = Path.Combine(savePath, rootName);

            if (Directory.Exists(fullDownloadPath))
            {
                Directory.Delete(fullDownloadPath, recursive: true);
            }
            else if (File.Exists(fullDownloadPath))
            {
                File.Delete(fullDownloadPath);
            }
        }

        public void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing) { }

            _InfoCollectionTask?.Wait();
            _InfoCollectionTask?.Dispose();
            _CancellationTokenSource?.Dispose();
            _Manager?.StopAsync();

            CleanUp();

            _Engine?.Dispose();

            _disposed = true;
        }
        #endregion
    }
}
