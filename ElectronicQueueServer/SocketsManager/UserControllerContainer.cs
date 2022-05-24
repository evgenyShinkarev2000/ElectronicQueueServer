namespace ElectronicQueueServer.SocketsManager
{
    public class UserControllerContainer
    {
        public SocketHandler SocketHandler { get; }
        public TicketMenager TicketMenager { get; }
        public UserControllerContainer(
            SocketHandler socketHandler,
            TicketMenager ticketMenager
            )
        {
            SocketHandler = socketHandler;
            TicketMenager = ticketMenager;
        }
    }
}
