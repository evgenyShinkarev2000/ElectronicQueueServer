using ElectronicQueueServer.Models;
using ElectronicQueueServer.SocketsManager;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicQueueServer.Handlers.WSUser
{
    public class WSUserHandler : SocketHandler
    {
        private readonly ILockManeger<WebSocket, string> _lockManeger;
        public WSUserHandler(ConnectionManager connectionManager, ILockManeger<WebSocket, string> lockManeger)
            : base(connectionManager)
        {
            this._lockManeger = lockManeger;
        }

        public override async Task OnConnection(WebSocket socket)
        {
            await base.OnConnection(socket);
        }

        public override async Task OnDisconnected(WebSocket webSocket)
        {
            this._lockManeger.UnlockAll(webSocket);
            await base.OnDisconnected(webSocket);
        }
    }
}
