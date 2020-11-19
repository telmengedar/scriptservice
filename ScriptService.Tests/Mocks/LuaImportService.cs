using System.Collections.Generic;
using ScriptService.Services;
using ScriptService.Services.JavaScript;

namespace ScriptService.Tests.Mocks {
    public class LuaImportService : IScriptImportService {
        Dictionary<string, IWorkableExecutor> scripts=new Dictionary<string,IWorkableExecutor>();
        Dictionary<string, IWorkableExecutor> workflows=new Dictionary<string,IWorkableExecutor>();
        Dictionary<string, object> hosts=new Dictionary<string,object>();

        public void AddHost(string name, object value) {
            hosts[name] = value;
        }
        public void AddScript(string name, IWorkableExecutor value) {
            scripts[name] = value;
        }
        public void AddWorkflow(string name, IWorkableExecutor value) {
            workflows[name] = value;
        }
        public object Host(string name) {
            return hosts[name];
        }

        public IWorkableExecutor Script(string name, int? revision) {
            return scripts[name];
        }

        public IWorkableExecutor Workflow(string name, int? revision) {
            return workflows[name];
        }

        public IScriptImportService Clone(WorkableLogger logger) {
            return this;
        }
    }
}