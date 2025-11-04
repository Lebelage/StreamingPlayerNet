using Microsoft.Extensions.DependencyInjection;
using StreamingPlayer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingPlayer.Services
{
    internal static class ServiceRegistrator
    {
        public static IServiceCollection AddServices(this IServiceCollection services) => services
            .AddSingleton<IFileDialog, FileDialogService>()
            .AddSingleton<IEventNotification, EventDispatcherService>()
            ;
    }
}
