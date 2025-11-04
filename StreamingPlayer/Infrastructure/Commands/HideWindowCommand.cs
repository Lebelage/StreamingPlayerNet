using StreamingPlayer.Infrastructure.Commands.Base;
using System.Reflection.Metadata;
using System.Windows;

namespace StreamingPlayer.Infrastructure.Commands
{
    internal class HideWindowCommand : Command
    {
        protected override void Execute(object? parameter)
        {
            Window? window = parameter as Window ?? App.FocusedWindow ?? App.ActivedWindow;
            if (window is not null)
                window.WindowState = WindowState.Minimized;
        }
    }
}
