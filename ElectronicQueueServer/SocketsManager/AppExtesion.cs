using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ElectronicQueueServer.SocketsManager
{
    public static class AppExtesion
    {
        // вместо middleware используется контроллер
        public static IApplicationBuilder MapSockets(this IApplicationBuilder appBuilder, PathString pathString, SocketHandler socketHandler)
        {
            return appBuilder.Map(pathString, request =>
            {
                // наверное, это можно сделать атрибутом route
                request.UseMiddleware<SocketMiddeleware>();
            });
        }
    }
}
