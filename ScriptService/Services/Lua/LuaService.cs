using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MoonSharp.Interpreter;
using NightlyCode.Scripting.Extensions;
using ScriptService.Services.Python;

namespace ScriptService.Services.Lua {
    /// <inheritdoc />
    public class LuaService : ILuaService {
        readonly IServiceProvider serviceprovider;
        readonly ITypeCreator typecreator;

        /// <summary>
        /// creates a new <see cref="LuaService"/>
        /// </summary>
        /// <param name="serviceprovider">provider for installed services</param>
        /// <param name="typecreator">service used to create new known types</param>
        public LuaService(IServiceProvider serviceprovider, ITypeCreator typecreator) {
            this.serviceprovider = serviceprovider;
            this.typecreator = typecreator;
            Script.GlobalOptions.Platform = new LuaPlatformAccessor();
            UserData.RegistrationPolicy = new LuaRegistrationPolicy();
        }

        object CreateType(string type, IDictionary<string, object> initializer) {
            object instance= typecreator.New(type);
            initializer?.FillType(instance);
            return instance;
        }
        
        /// <inheritdoc />
        public object Execute(string code, IDictionary<string, object> variables) {
            Script script=new Script();
            
            foreach (KeyValuePair<string, object> value in variables)
                script.Globals[value.Key] = value.Value;
            script.Globals["load"] = new LuaImportService(serviceprovider, null);
            script.Globals["await"] = (Func<Task, object>) Helpers.Tasks.AwaitTask;
            script.Globals["new"] = (Func<string, IDictionary<string, object>, object>) CreateType;
            
            DynValue result = script.DoString(code);
            switch (result.Type) {
            case DataType.Boolean:
                return result.Boolean;
            case DataType.Number:
                return result.Number;
            case DataType.String:
                return result.String;
            case DataType.UserData:
                return result.UserData.Object;
            }
            return null;
        }
    }
}