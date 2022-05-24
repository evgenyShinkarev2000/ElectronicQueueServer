using ElectronicQueueServer.Handlers;
using System;

namespace ElectronicQueueServer.SocketsManager
{
    public interface IAccessGuard
    {
        public string Role { get; set; }
        public bool HasAccess(IWSController controller);
        public bool HasAccess(Delegate method);
    }
}
