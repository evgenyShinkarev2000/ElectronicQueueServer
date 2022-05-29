using ElectronicQueueServer.Models;
using ElectronicQueueServer.SocketsManager;
using Newtonsoft.Json.Linq;
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
        private Models.User _data;
        public User(IWSControllerFactory controllerFactory)
        {
            this._socketHandler = controllerFactory.SocketHandler;
            this._appDB = controllerFactory.AppDB;
        }

        [RoleValidator(new[] { "ADMIN" })]
        public async Task Delete()
        {
            await _appDB.DeleteUser(this._data);
            await this._socketHandler.SendMessageToAll(
                new WSMessageToClient(new[] { "delete", "user" }, this._data));
        }

        public Task Get()
        {
            throw new System.NotImplementedException();
        }

        [RoleValidator(new[] { "ADMIN" })]
        public async Task Handle(IEnumerable<string> instructions, object data)
        {
            this._data = (data as JObject).ToObject<Models.User>();
            var handler = new Dictionary<string, Func<Task>>()
            {
                {"post", () => this.Post() },
                {"update", () => this.Update() },
                {"delete", () => this.Delete() }
            };

            await handler[instructions.First()]();
        }

        [RoleValidator(new[] { "ADMIN" })]
        public async Task Post()
        {
            await this._appDB.AddUser(this._data as Models.User);
            await this._socketHandler.SendMessageToAll(
                new WSMessageToClient(new[] { "post", "user" }, this._data as Models.User));
        }

        [RoleValidator(new[] { "ADMIN" })]
        public async Task Update()
        {
            await _appDB.ReplaceUser(this._data as Models.User);
            await this._socketHandler.SendMessageToAll(
                new WSMessageToClient(new[] { "update", "user" }, this._data as Models.User));
        }
    }
}
