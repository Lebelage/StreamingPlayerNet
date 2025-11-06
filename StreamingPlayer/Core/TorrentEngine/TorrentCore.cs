using MonoTorrent;
using MonoTorrent.Client;

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
        private CancellationTokenSource _CancellationTokenSource;
        #endregion

        #region Methods
        public async Task LoadTorrent(string torrentFilePath, string downloadDirectory)
        {
            _DownloadPath = torrentFilePath;

            var torrent = await Torrent.LoadAsync(torrentFilePath);
            _Manager = await _Engine.AddAsync(torrent, downloadDirectory);

            _Manager.TorrentStateChanged += (s, e) => TorrentStateChanged?.Invoke(this, e);

            // SetupSequentialDownload();

            await _Manager.StartAsync();
        }

        //private void SetupSequentialDownload()
        //{
        //    if (_Manager == null) return;

        //    // Получаем файлы в торренте
        //    var files = _Manager.Torrent.Files;

        //    // Сортируем файлы для последовательной загрузки
        //    for (int i = 0; i < files.Count; i++)
        //    {
        //        // Высокий приоритет для первых файлов, низкий для остальных
        //        var priority = i == 0 ? Priority.Immediate : Priority.Low;
        //        _Manager.SetFilePriority(files[i], priority);
        //    }

        //    // Обработчик прогресса загрузки для динамического изменения приоритетов
        //    _Manager.PieceHashed += OnPieceHashed;
        //}

        private void InfoCollection(CancellationToken token, uint timeout = 2000)
        {
            Task.Run(() =>
            {

                float lastState = 0;

                while (!token.IsCancellationRequested)
                {
                    float newState = _Engine.TotalDownloadRate / 1024f;

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


        public TorrentCore()
        {
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
