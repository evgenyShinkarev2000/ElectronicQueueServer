using ElectronicQueueServer.Models;
using ElectronicQueueServer.SocketsManager;
using System;
using System.Net.WebSockets;

namespace ElectronicQueueServer.Handlers
{
    public class ProtectedControllerFactory: IWSControllerFactory
    {
        public string Role { get; set; }
        public WebSocket WebSocket { get; set; }
        public SocketHandler SocketHandler { get; set; }
        public ILockManeger<WebSocket, string> LockManeger { get; set; }
        public AppDB AppDB { get ; set; }
        

        public IWSController CreateInstacne<T>() where T : class, IWSController
        {
            return new ProtectedController<T>(this);
        }
    }
}
