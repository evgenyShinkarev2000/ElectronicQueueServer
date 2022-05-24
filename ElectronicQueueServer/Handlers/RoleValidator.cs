using System;
using System.Collections.Generic;

namespace ElectronicQueueServer.Handlers
{
    public class RoleValidatorAttribute: Attribute
    {
        public HashSet<string> Roles { get; }
        public RoleValidatorAttribute(string[] roles)
        {
            Roles = new HashSet<string>(roles);
        }
    }
}
