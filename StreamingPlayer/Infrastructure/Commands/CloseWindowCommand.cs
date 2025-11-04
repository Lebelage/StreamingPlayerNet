using StreamingPlayer.Infrastructure.Commands.Base;
using System.Windows;

namespace StreamingPlayer.Infrastructure.Commands
{
    internal class CloseWindowCommand : Command
    {
        protected override void Execute(object? parameter) => (parameter as Window ?? App.FocusedWindow ?? App.ActivedWindow)?.Close();
    }
}
