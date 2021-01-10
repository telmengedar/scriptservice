using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using ScriptService.Dto.Tasks;
using ScriptService.Services;
using ScriptService.Services.Workflows;
using ScriptService.Services.Workflows.Nodes;

namespace ScriptService.Tests.Mocks {
    public class NodeTest {

        public static Task<object> Execute(IInstanceNode node, IDictionary<string, object> variables=null) {
            variables ??= new Dictionary<string, object>();
            return node.Execute(new WorkflowInstanceState(new WorkableLogger(new NullLogger<NodeTest>(), new WorkableTask()), new StateVariableProvider(variables), s => null, null), CancellationToken.None);
        }
    }
}