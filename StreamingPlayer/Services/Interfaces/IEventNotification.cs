
namespace StreamingPlayer.Services.Interfaces
{
    internal interface IEventNotification
    {
        public event EventHandler<string?>? TorrentFileSelected;
        void Invoke(string name, object sender, params object?[] args);
    }
}
