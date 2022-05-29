using ElectronicQueueServer.Handlers;
using ElectronicQueueServer.Handlers.WSUser;
using Microsoft.Extensions.DependencyInjection;
using System.Net.WebSockets;

namespace ElectronicQueueServer.SocketsManager
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddWebSocketManeger(this IServiceCollection services)
        {
            services.AddTransient<ConnectionManager>();
            services.AddTransient<TicketMenager>();
            services.AddTransient<ILockManeger<WebSocket, string>, LockManeger<WebSocket, string>>();
            services.AddTransient<IWSControllerFactory, ProtectedControllerFactory>();
            services.AddSingleton<UserControllerContainer>(); 

            return services;
        }
    }
}
