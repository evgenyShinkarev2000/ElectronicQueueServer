using ElectronicQueueServer.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ElectronicQueueServer.SocketsManager
{
    public abstract class SocketHandler
    {
        public ConnectionManager ConnectionsManager { get; }

        public SocketHandler(ConnectionManager connectionsManager)
        {
            ConnectionsManager = connectionsManager;
        }

        public virtual async Task OnConnection(WebSocket socket)
        {
            await Task.Run(() => ConnectionsManager.AddSocket(socket));
        }

        public virtual async Task OnDisconnected(WebSocket webSocket)
        {
            await ConnectionsManager.RemoveSocket(ConnectionsManager.GetId(webSocket));
        }

        public async Task SendMessage(WebSocket webSocket, string message)
        {
            if (webSocket.State != WebSocketState.Open)
            {
                return;
            }

            await webSocket.SendAsync(
                Encoding.UTF8.GetBytes(message),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }

        public async Task SendMessage(string id, string message)
        {
            var webSocket = ConnectionsManager.GetSocketByID(id);
            if (webSocket.State != WebSocketState.Open)
            {
                return;
            }

            await this.SendMessage(webSocket, message);
        }

        public async Task SendMessage(WebSocket webSocket, WSMessageToClient message)
        {
            var messageString = JsonConvert.SerializeObject(message);
            await this.SendMessage(webSocket, messageString);
        }

        public async Task SendMessageToAll(string message)
        {
            foreach(var connection in this.ConnectionsManager.GetAllConnections())
            {
                await this.SendMessage(connection.Value, message);
            }
            
        }

        public async Task SendMessageToAll(WSMessageToClient objMessage)
        {
            var message = JsonConvert.SerializeObject(objMessage);
            await this.SendMessageToAll(message);
        }

        public async Task SendMessageToAllExcept(WebSocket webSocket, string message)
        {
            foreach (var connection in this.ConnectionsManager.GetAllConnections().Where(pair => pair.Value != webSocket))
            {
                await this.SendMessage(connection.Value, message);
            }
        }
        public async Task SendMessageToAllExcept(string id, string message)
        {
            foreach (var connection in this.ConnectionsManager.GetAllConnections().Where(pair => pair.Key != id))
            {
                await this.SendMessage(connection.Value, message);
            }
        }
        public async Task SendMessageToAllExcept(WebSocket webSocket, WSMessageToClient data)
        {
            await this.SendMessageToAllExcept(webSocket, JsonConvert.SerializeObject(data));
        }
    }
}
