using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace ElectronicQueueServer.SocketsManager
{
    public class ConnectionManager
    {
        readonly private ConcurrentDictionary<string, WebSocket> _connections = new ConcurrentDictionary<string, WebSocket>();
        public WebSocket GetSocketByID(string id)
        {
            return this._connections.TryGetValue(id, out WebSocket ws) ? ws : null;
        }

        public ConcurrentDictionary<string, WebSocket> GetAllConnections()
        {
            return this._connections;
        }

        public string GetId(WebSocket webSocket)
        {
            return this._connections.FirstOrDefault(pair => pair.Value == webSocket).Key;
        }

        public async Task RemoveSocket(string id)
        {
            this._connections.TryRemove(id, out WebSocket ws);
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "сокет закрыт вызовом метода RemoveSocket", CancellationToken.None);
        } 

        public void AddSocket(WebSocket webSocket)
        {
            if (webSocket == null)
            {
                throw new Exception();
            }

            this._connections.TryAdd(Guid.NewGuid().ToString("N"), webSocket);
        }
    }
}
