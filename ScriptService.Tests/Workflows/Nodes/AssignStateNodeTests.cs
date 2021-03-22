using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NightlyCode.Scripting.Parser;
using NUnit.Framework;
using ScriptService.Dto;
using ScriptService.Dto.Workflows.Nodes;
using ScriptService.Services.Scripts;
using ScriptService.Services.Workflows;
using ScriptService.Services.Workflows.Nodes;

namespace ScriptService.Tests.Workflows.Nodes {
    
    [TestFixture, Parallelizable]
    public class AssignStateNodeTests {

        [TestCase(VariableOperation.Assign, 11)]
        [TestCase(VariableOperation.Add, 11)]
        [TestCase(VariableOperation.Subtract, -11)]
        [TestCase(VariableOperation.Divide, 0)]
        [TestCase(VariableOperation.Multiply, 0)]
        [TestCase(VariableOperation.Modulo, 0)]
        [TestCase(VariableOperation.BitAnd, 0)]
        [TestCase(VariableOperation.BitOr, 11)]
        [TestCase(VariableOperation.BitXor, 11)]
        [TestCase(VariableOperation.ShiftLeft, 0)]
        [TestCase(VariableOperation.ShiftRight, 0)]
        [TestCase(VariableOperation.RollLeft, 0)]
        [TestCase(VariableOperation.RollRight, 0)]
        [Parallelizable]
        public async Task AssignWithOperationVariableNotExisting(VariableOperation operation, object expected) {
            Mock<IInstanceNode> instancenode=new Mock<IInstanceNode>();
            instancenode.Setup(s => s.Execute(It.IsAny<WorkflowInstanceState>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(11);
            Mock<IScriptCompiler> compiler=new Mock<IScriptCompiler>();
            compiler.Setup(s => s.CompileCode(It.IsAny<string>(), ScriptLanguage.NCScript)).Returns<string, ScriptLanguage>((code, language) => new ScriptParser().Parse(code));
            
            AssignStateNode node=new AssignStateNode(instancenode.Object, "result", operation, compiler.Object);
            
            Dictionary<string, object> variables=new Dictionary<string, object>();
            await node.Execute(new WorkflowInstanceState(null, new StateVariableProvider(variables), s => null, null, null, false), CancellationToken.None);

            Assert.That(variables.ContainsKey("result"));
            Assert.AreEqual(expected, variables["result"]);
        }
        
        [TestCase(VariableOperation.Assign, 11)]
        [TestCase(VariableOperation.Add, 18)]
        [TestCase(VariableOperation.Subtract, -4)]
        [TestCase(VariableOperation.Divide, 0)]
        [TestCase(VariableOperation.Multiply, 77)]
        [TestCase(VariableOperation.Modulo, 7)]
        [TestCase(VariableOperation.BitAnd, 3)]
        [TestCase(VariableOperation.BitOr, 15)]
        [TestCase(VariableOperation.BitXor, 12)]
        [TestCase(VariableOperation.ShiftLeft, 14336)]
        [TestCase(VariableOperation.ShiftRight, 0)]
        [TestCase(VariableOperation.RollLeft, 14336)]
        [TestCase(VariableOperation.RollRight, 14680064)]
        [Parallelizable]
        public async Task AssignWithOperationExistingVariable(VariableOperation operation, object expected) {
            Mock<IInstanceNode> instancenode=new Mock<IInstanceNode>();
            instancenode.Setup(s => s.Execute(It.IsAny<WorkflowInstanceState>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(11);
            Mock<IScriptCompiler> compiler=new Mock<IScriptCompiler>();
            compiler.Setup(s => s.CompileCode(It.IsAny<string>(), ScriptLanguage.NCScript)).Returns<string, ScriptLanguage>((code, language) => new ScriptParser().Parse(code));
            
            AssignStateNode node=new AssignStateNode(instancenode.Object, "result", operation, compiler.Object);

            Dictionary<string, object> variables = new Dictionary<string, object> {
                ["result"] = 7
            };
            await node.Execute(new WorkflowInstanceState(null, new StateVariableProvider(variables), s => null, null, null, false), CancellationToken.None);

            Assert.That(variables.ContainsKey("result"));
            Assert.AreEqual(expected, variables["result"]);
        }
    }
}