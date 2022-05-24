using ElectronicQueueServer.Models;
using ElectronicQueueServer.SocketsManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicQueueServer.Handlers.WSUser
{
    public class User : IWSController
    {
        private readonly SocketHandler _socketHandler;
        private readonly AppDB _appDB;
        private object _data;
        public User(IWSControllerFactory controllerFactory)
        {
            this._socketHandler = controllerFactory.SocketHandler;
            this._appDB = controllerFactory.AppDB;
        }
        public async Task Delete()
        {
            await _appDB.DeleteUser(this._data as Models.User);
            await this._socketHandler.SendMessageToAll(
                new WSMessageToClient(new[] {"user", "delete"}, this._data as Models.User));
        }

        public Task Get()
        {
            throw new System.NotImplementedException();
        }

        public async Task Handle(IEnumerable<string> instructions, object data)
        {
            this._data = data;
            var handler = new Dictionary<string, Func<Task>>()
            {
                {"post", () => this.Post() },
                {"update", () => this.Update() },
                {"delete", () => this.Delete() }
            };

            await handler[instructions.First()]();
        }

        public async Task Post()
        {
            await this._appDB.AddUser(this._data as Models.User);
            await this._socketHandler.SendMessageToAll(
                new WSMessageToClient(new[] { "user", "post" }, this._data as Models.User));
        }

        public async Task Update()
        {
            await _appDB.ReplaceUser(this._data as Models.User);
            await this._socketHandler.SendMessageToAll(
                new WSMessageToClient(new[] {"user", "update"}, this._data as Models.User));
        }
    }
}
