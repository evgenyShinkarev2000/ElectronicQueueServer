using ElectronicQueueServer.Handlers;
using ElectronicQueueServer.Handlers.Factory;
using Microsoft.Extensions.DependencyInjection;

namespace ElectronicQueueServer.SocketsManager
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddWebSocketManeger(this IServiceCollection services)
        {
            services.AddTransient<ConnectionManager>();
            services.AddTransient<TicketMenager>();
            services.AddTransient<SocketHandler>();
            services.AddTransient<IWSControllerFactory, ProtectedControllerFactory>();
            services.AddSingleton<UserControllerContainer>(); 

            return services;
        }
    }
}
