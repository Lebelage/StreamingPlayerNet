using StreamingPlayer.Infrastructure.Commands.Base;
using System.Windows;

namespace StreamingPlayer.Infrastructure.Commands
{
    internal class NormalizeWindowCommand : Command
    {
        protected override void Execute(object? parameter)
        {
            Window? window = parameter as Window ?? App.FocusedWindow ?? App.ActivedWindow;
            if (window is not null)
                window.WindowState = WindowState.Normal;
        }
    }
}
