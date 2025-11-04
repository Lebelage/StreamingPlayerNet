using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using StreamingPlayer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace StreamingPlayer.Services
{
    internal class FileDialogService : IFileDialog
    {
        private IEventNotification eventNotification;
        public Task OpenFileAsync(string filter = "Torrent files (*.torrent)|*.torrent|All files (*.*)|*.*", string title = "Open torrent file")
        {
            return Task.Run(() =>
            {
                var fileDialog = new OpenFileDialog();
                fileDialog.Filter = filter;
                fileDialog.Title = title;
                fileDialog.Multiselect = false;

                bool result = fileDialog.ShowDialog() ?? false;

                if (result)
                    eventNotification.Invoke(nameof(IEventNotification.TorrentFileSelected), this, fileDialog.FileName);
            });
        }

        public FileDialogService() 
        {
            eventNotification = App.Services.GetRequiredService<IEventNotification>();
        }
    }
}
