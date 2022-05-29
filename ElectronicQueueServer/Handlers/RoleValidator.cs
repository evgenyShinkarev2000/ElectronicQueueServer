using System;
using System.Collections.Generic;

namespace ElectronicQueueServer.Handlers
{
    public class RoleValidatorAttribute: Attribute
    {
        // не работает с методами, можно попробовать TypeBuilder
        public HashSet<string> Roles { get; }
        public RoleValidatorAttribute(string[] roles)
        {
            Roles = new HashSet<string>(roles);
        }
    }
}
