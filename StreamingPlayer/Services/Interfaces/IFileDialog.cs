
namespace StreamingPlayer.Services.Interfaces
{
    internal interface IFileDialog
    {
        public Task OpenFileAsync(string filter = "Torrent files (*.torrent)|*.torrent|All files (*.*)|*.*", string title = "Open torrent file");
    } 
}
