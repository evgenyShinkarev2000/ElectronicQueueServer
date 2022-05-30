using ElectronicQueueServer.Models;
using ElectronicQueueServer.SocketsManager;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace ElectronicQueueServer.Handlers.WSUser
{
    public class EditRight : IWSController
    {
        private readonly ILockManeger<WebSocket, string> _lockManeger;
        private readonly WebSocket _webSocket;
        private readonly SocketHandler _handler;
        private LockedItem _lockedItem;
        public EditRight(IWSControllerFactory controllerFactory)
        {
            this._lockManeger = controllerFactory.LockManeger;
            this._webSocket = controllerFactory.WebSocket;
            this._handler = controllerFactory.SocketHandler;
        }

        [RoleValidator(new[] { "ADMIN" })]
        public async Task Delete()
        {
            var wasUnlocked = this._lockManeger.Unlock(_webSocket, _lockedItem.ItemId);
            if (wasUnlocked)
            {
                await this._handler.SendMessageToAllExcept(_webSocket, new WSMessageToClient(
                    new[] { "update", "editRight" },
                    new LockedItem(_lockedItem.ItemId, LockedItem.LockedStatus.Free)));
            }
        }

        [RoleValidator(new[] { "ADMIN" })]
        public Task Get()
        {
            throw new System.NotImplementedException();
        }

        [RoleValidator(new[] { "ADMIN" })]
        public async Task Handle(IEnumerable<string> instructions, object data)
        {
            var handler = new Dictionary<string, System.Func<Task>>()
            {
                {"post", () => this.Post()},
                {"delete", () => this.Delete()}
            };

            this._lockedItem = (data as JObject).ToObject<LockedItem>();
            await handler[instructions.First()]();
        }

        public async Task Post()
        {
            var canUserEdit = this._lockManeger.TryLock(_webSocket, this._lockedItem.ItemId);
            await this._handler.SendMessage(_webSocket, new WSMessageToClient(new[] { "post", "editRight" }, canUserEdit));
            if (canUserEdit)
            {
                await this._handler.SendMessageToAllExcept(_webSocket, new WSMessageToClient(
                    new[] { "update", "editRight" },
                    new LockedItem(this._lockedItem.ItemId, LockedItem.LockedStatus.Lock)));
            }
        }

        public Task Update()
        {
            throw new System.NotImplementedException();
        }
    }
}
