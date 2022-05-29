using ElectronicQueueServer.SocketsManager;
using System.Net.WebSockets;

namespace ElectronicQueueServer.Handlers.WSUser
{
    public class UserControllerContainer
    {
        public WSUserHandler SocketHandler { get; }
        public TicketMenager TicketMenager { get; }
        public ConnectionManager ConnectionManager {get;}
        public ILockManeger<WebSocket, string> LockManeger { get; }
        public UserControllerContainer(
            TicketMenager ticketMenager,
            ILockManeger<WebSocket, string> lockManeger,
            ConnectionManager connectionManager
            )
        {
            SocketHandler = new WSUserHandler(connectionManager, lockManeger);
            TicketMenager = ticketMenager;
            LockManeger = lockManeger;
        }
    }
}
