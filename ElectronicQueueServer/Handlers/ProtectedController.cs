using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace ElectronicQueueServer.Handlers
{
    public class ProtectedController<T> : IWSController where T : class, IWSController
    {
        public string Role { get; private set; }
        private readonly IWSController _originController;
        public ProtectedController(IWSControllerFactory controllerFactory)
        {
            Role = controllerFactory.Role;
            _originController = (T)Activator.CreateInstance(typeof(T), controllerFactory);
        }

        public async Task Handle(IEnumerable<string> instructions, object data)
        {
            CheckAccessClass();
            await _originController.Handle(instructions, data);
        }

        public async Task Delete()
        {
            CheckAccessMethod(nameof(this.Delete));
            await _originController.Delete();
        }

        public async Task Get()
        {
            CheckAccessMethod(nameof(this.Get));
            await _originController.Get();
        }

        public async Task Post()
        {
            CheckAccessMethod(nameof(this.Post));
            await _originController.Post();
        }

        public async Task Update()
        {
            CheckAccessMethod(nameof(this.Update));
            await _originController.Update();
        }

        private void CheckAccessClass()
        {
            var type = typeof(T);
            foreach (var attribute in type.GetCustomAttributes(false))
            {
                if (attribute is RoleValidatorAttribute)
                {
                    if ((attribute as RoleValidatorAttribute).Roles.Contains(Role))
                    {
                        return;
                    }

                    throw new Exception($"{Role} havn't access to {type.Name}");
                }
            }

            throw new Exception($"origin class {type.Name} doesn't contain attribute {nameof(RoleValidatorAttribute)}");
        }

        private void CheckAccessMethod(string methodName)
        {
            var type = typeof(T);
            var method = type.GetMethod(methodName);
            if (method == null)
            {
                throw new Exception($"method {method.Name} from {type.Name} doesn't exist");
            }

            foreach (var attribute in method.GetCustomAttributes(false))
            {
                if (attribute is RoleValidatorAttribute)
                {
                    if ((attribute as RoleValidatorAttribute).Roles.Contains(Role))
                    {
                        return;
                    }

                    throw new Exception($"{Role} havn't access to {method.Name}");
                }
            }

            throw new Exception($"origin class {type.Name} doesn't contain method {method.Name} " +
                $"with attribute {nameof(RoleValidatorAttribute)}");
        }
    }
}
