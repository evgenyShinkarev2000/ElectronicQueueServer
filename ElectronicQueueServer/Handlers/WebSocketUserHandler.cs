using ElectronicQueueServer.SocketsManager;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicQueueServer.Handlers
{
    public class WebSocketUserHandler : SocketHandler
    {
        public WebSocketUserHandler(ConnectionManager connectionManager): base(connectionManager)
        {

        }

        public override async Task OnConnection(WebSocket socket)
        {
            await base.OnConnection(socket);
            var socketId = this.ConnectionsManager.GetId(socket);
            await this.SendMessageToAll($"user with id {socketId} joined");
        }
        public override async Task Receive(WebSocket webSocket, WebSocketReceiveResult result, byte[] buffer)
        {
            var socketID = this.ConnectionsManager.GetId(webSocket);
            var message = $"user {socketID} said {Encoding.UTF8.GetString(buffer, 0, result.Count)}";
            await this.SendMessageToAll(message);
        }
    }
}
