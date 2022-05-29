using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;

namespace ElectronicQueueServer.SocketsManager
{
    public class LockManeger<T, U>: ILockManeger<T, U>
    {
        private readonly ConcurrentDictionary<T, HashSet<U>> _socketLockItems 
            = new ConcurrentDictionary<T, HashSet<U>>();
        private readonly HashSet<U> _lockIdSet = new HashSet<U>();

        public bool TryLock(T holder, U lockedId)
        {
            if (this._lockIdSet.Contains(lockedId))
            {
                return false;
            }

            if (this._socketLockItems.ContainsKey(holder))
            {
                this._socketLockItems[holder].Add(lockedId);
            }
            else
            {
                this._socketLockItems.TryAdd(holder, new HashSet<U>() { lockedId });
            }

            this._lockIdSet.Add(lockedId);

            return true;
        }

        public bool Unlock(T holder, U lockedId)
        {
            if (this._socketLockItems.TryGetValue(holder, out var lockIdSet) && lockIdSet.Remove(lockedId))
            {
                this._lockIdSet.Remove(lockedId);

                return true;
            }

            return false;
        }
        public bool UnlockAll(T holder)
        {
            if (this._socketLockItems.TryRemove(holder, out var lockIdSet))
            {
                foreach(var lockId in lockIdSet)
                {
                    this._lockIdSet.Remove(lockId);
                }

                return true;
            }

            return false;
        }

        public bool IsLocked(U lockedId)
        {
            return this._lockIdSet.Contains(lockedId);
        }
    }
}
