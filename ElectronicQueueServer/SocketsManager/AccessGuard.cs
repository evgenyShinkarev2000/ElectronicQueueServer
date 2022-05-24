using ElectronicQueueServer.Handlers;
using System;

namespace ElectronicQueueServer.SocketsManager
{
    public class AccessGuard : IAccessGuard
    {
        public string Role { get; set; }

        public bool HasAccess(IWSController controller) => this.HasAccess(controller);

        public bool HasAccess(Delegate method) => this.HasAccess(method);

        private bool HasAccess(object obj)
        {
            if (Role == null)
            {
                throw new Exception("не указана роль");
            }
            var type = obj.GetType();
            foreach (var attribute in type.GetCustomAttributes(false))
            {
                if (attribute is RoleValidatorAttribute)
                {
                    return (attribute as RoleValidatorAttribute).Roles.Contains(this.Role);
                }
            }

            throw new Exception($"origin {type.Name} doesn't contain attribute {nameof(RoleValidatorAttribute)}");
        }
    }
}
