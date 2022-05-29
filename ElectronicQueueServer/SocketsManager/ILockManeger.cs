using System;
using System.Net.WebSockets;

namespace ElectronicQueueServer.SocketsManager
{
    public interface ILockManeger<T, U>
    {
        public bool TryLock(T holder, U lockedId);
        public bool Unlock(T holder, U lockedId);
        public bool UnlockAll(T holder);
        public bool IsLocked(U lockedId);
    }
}
