using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicQueueServer.Handlers.WSUser
{
    public class Main : IWSController
    {
        private readonly IWSControllerFactory _controllerFactory;
        public Main(IWSControllerFactory controllerFactory)
        {
            this._controllerFactory = controllerFactory;
        }
        public Task Delete()
        {
            throw new System.NotImplementedException();
        }

        public Task Get()
        {
            throw new System.NotImplementedException();
        }

        public async Task Handle(IEnumerable<string> instructions, object data)
        {
            var handlers = new Dictionary<string, Func<IWSController>>()
            {
                {"user", () => this._controllerFactory.CreateInstacne<User>()},
                {"allUser", () => this._controllerFactory.CreateInstacne<AllUser>() },
                {"editRight", () => this._controllerFactory.CreateInstacne<EditRight>() }
            };

            var instruction = instructions.Last();
            handlers.TryGetValue(instruction, out var controllerBuilder);
            if (controllerBuilder == null)
            {
                throw new Exception($"неизвестный контроллер {instruction}");
            }
            var controller = controllerBuilder();

            await controller.Handle(instructions.SkipLast(1), data);
        }

        public Task Post()
        {
            throw new System.NotImplementedException();
        }

        public Task Update()
        {
            throw new System.NotImplementedException();
        }
    }
}
