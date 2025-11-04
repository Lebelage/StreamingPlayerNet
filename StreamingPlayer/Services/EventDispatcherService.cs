using StreamingPlayer.Services.Interfaces;
using System.Windows.Threading;

namespace StreamingPlayer.Services
{
    internal class EventDispatcherService : IEventNotification
    {
        public event EventHandler<string?>? TorrentFileSelected;

        private readonly Dispatcher _Dispatcher = Dispatcher.CurrentDispatcher;
        public void Invoke(string name, object sender, params object?[] args)
        {
            if (GetType().GetField(name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.GetValue(this) is not MulticastDelegate md)
                return;

            args = [sender, .. args];
            foreach (Delegate d in md.GetInvocationList())
                _Dispatcher.BeginInvoke(() => d.Method.Invoke(d.Target, args));
        }
    }

}
