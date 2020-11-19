using System;
using MoonSharp.Interpreter.Interop;
using MoonSharp.Interpreter.Interop.RegistrationPolicies;

namespace ScriptService.Services.Lua {
    
    /// <summary>
    /// type registration policy for lua objects
    /// </summary>
    public class LuaRegistrationPolicy : IRegistrationPolicy {
        
        public IUserDataDescriptor HandleRegistration(IUserDataDescriptor newDescriptor, IUserDataDescriptor oldDescriptor) {
            return newDescriptor ?? oldDescriptor;
        }

        /// <inheritdoc />
        public bool AllowTypeAutoRegistration(Type type) => true;
    }
}