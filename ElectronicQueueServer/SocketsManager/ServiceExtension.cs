using ElectronicQueueServer.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace ElectronicQueueServer.SocketsManager
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddWebSocketManeger(this IServiceCollection services)
        {
            services.AddTransient<ConnectionManager>();
            services.AddTransient<TicketMenager>();
            services.AddSingleton<WSUserHandler>(); 

            return services;
        }
    }
}
