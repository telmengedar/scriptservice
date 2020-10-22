using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NightlyCode.Scripting.Parser;
using NUnit.Framework;
using ScriptService.Dto;
using ScriptService.Dto.Workflows;
using ScriptService.Dto.Workflows.Nodes;
using ScriptService.Errors;
using ScriptService.Services.Scripts;
using ScriptService.Services.Workflows.Nodes;

namespace ScriptService.Tests.Workflows.Nodes {
    
    [TestFixture, Parallelizable]
    public class StartNodeTests {

        [Test, Parallelizable]
        public async Task NoParameters() {
            Mock<IScriptCompiler> compiler=new Mock<IScriptCompiler>();
            compiler.Setup(s => s.CompileCode(It.IsAny<string>(), ScriptLanguage.NCScript)).Returns<string, ScriptLanguage>((code, language) => new ScriptParser().Parse(code));
            compiler.Setup(s => s.CompileCodeAsync(It.IsAny<string>(), ScriptLanguage.NCScript)).Returns<string, ScriptLanguage>((code, language) => new ScriptParser().ParseAsync(code));
            
            StartNode node=new StartNode("Start", new StartParameters(), compiler.Object);

            Dictionary<string, object> variables = new Dictionary<string, object>();
            await node.Execute(null, null, variables, CancellationToken.None);
        }

        [Test, Parallelizable]
        public async Task ForcedParameter() {
            Mock<IScriptCompiler> compiler=new Mock<IScriptCompiler>();
            compiler.Setup(s => s.CompileCode(It.IsAny<string>(), ScriptLanguage.NCScript)).Returns<string, ScriptLanguage>((code, language) => new ScriptParser().Parse(code));
            compiler.Setup(s => s.CompileCodeAsync(It.IsAny<string>(), ScriptLanguage.NCScript)).Returns<string, ScriptLanguage>((code, language) => new ScriptParser().ParseAsync(code));
            
            StartNode node=new StartNode("Start", new StartParameters {
                Parameters = new[] {
                    new ParameterDeclaration {
                        Name = "input",
                        Type = "int"
                    }
                }
            }, compiler.Object);

            Dictionary<string, object> variables = new Dictionary<string, object>() {
                ["input"] = "7"
            };
            await node.Execute(null, null, variables, CancellationToken.None);

            Assert.AreEqual(7, variables["input"]);
        }
        
        [Test, Parallelizable]
        public void ForcedParameterNotProvided() {
            Mock<IScriptCompiler> compiler=new Mock<IScriptCompiler>();
            compiler.Setup(s => s.CompileCode(It.IsAny<string>(), ScriptLanguage.NCScript)).Returns<string, ScriptLanguage>((code, language) => new ScriptParser().Parse(code));
            compiler.Setup(s => s.CompileCodeAsync(It.IsAny<string>(), ScriptLanguage.NCScript)).Returns<string, ScriptLanguage>((code, language) => new ScriptParser().ParseAsync(code));
            
            StartNode node=new StartNode("Start", new StartParameters {
                Parameters = new[] {
                    new ParameterDeclaration {
                        Name = "input",
                        Type = "int"
                    }
                }
            }, compiler.Object);

            Dictionary<string, object> variables = new Dictionary<string, object>();
            Assert.ThrowsAsync<WorkflowException>(() => node.Execute(null, null, variables, CancellationToken.None));
        }
        
        [Test, Parallelizable]
        public void ForcedParameterNullToValueType() {
            Mock<IScriptCompiler> compiler=new Mock<IScriptCompiler>();
            compiler.Setup(s => s.CompileCode(It.IsAny<string>(), ScriptLanguage.NCScript)).Returns<string, ScriptLanguage>((code, language) => new ScriptParser().Parse(code));
            compiler.Setup(s => s.CompileCodeAsync(It.IsAny<string>(), ScriptLanguage.NCScript)).Returns<string, ScriptLanguage>((code, language) => new ScriptParser().ParseAsync(code));
            
            StartNode node=new StartNode("Start", new StartParameters {
                Parameters = new[] {
                    new ParameterDeclaration {
                        Name = "input",
                        Type = "int",
                        Default = "null"
                    }
                }
            }, compiler.Object);

            Dictionary<string, object> variables = new Dictionary<string, object>();
            Assert.ThrowsAsync<WorkflowException>(() => node.Execute(null, null, variables, CancellationToken.None));
        }
        
        [Test, Parallelizable]
        public async Task ForcedParameterDefaultArray() {
            Mock<IScriptCompiler> compiler=new Mock<IScriptCompiler>();
            compiler.Setup(s => s.CompileCode(It.IsAny<string>(), ScriptLanguage.NCScript)).Returns<string, ScriptLanguage>((code, language) => new ScriptParser().Parse(code));
            compiler.Setup(s => s.CompileCodeAsync(It.IsAny<string>(), ScriptLanguage.NCScript)).Returns<string, ScriptLanguage>((code, language) => new ScriptParser().ParseAsync(code));
            
            StartNode node=new StartNode("Start", new StartParameters {
                Parameters = new[] {
                    new ParameterDeclaration {
                        Name = "input",
                        Type = "int[]",
                        Default = "[1,2,3]"
                    }
                }
            }, compiler.Object);

            Dictionary<string, object> variables = new Dictionary<string, object>();
            await node.Execute(null, null, variables, CancellationToken.None);

            IEnumerable<int> input = variables["input"] as IEnumerable<int>;
            Assert.NotNull(input);
            Assert.That(new[] {1, 2, 3}.SequenceEqual(input));
        }
        
        [Test, Parallelizable]
        public async Task ForcedParameterCustomType() {
            IScriptParser parser=new ScriptParser();
            parser.Types.AddType<Transition>();
            
            Mock<IScriptCompiler> compiler=new Mock<IScriptCompiler>();
            compiler.Setup(s => s.CompileCode(It.IsAny<string>(), ScriptLanguage.NCScript)).Returns<string, ScriptLanguage>((code, language) => parser.Parse(code));
            compiler.Setup(s => s.CompileCodeAsync(It.IsAny<string>(), ScriptLanguage.NCScript)).Returns<string, ScriptLanguage>((code, language) => parser.ParseAsync(code));
            
            StartNode node=new StartNode("Start", new StartParameters {
                Parameters = new[] {
                    new ParameterDeclaration {
                        Name = "input",
                        Type = "transition[]",
                        Default = "[{\"Log\": \"\\\"Hello there\\\"\"}]"
                    }
                }
            }, compiler.Object);

            Dictionary<string, object> variables = new Dictionary<string, object>();
            await node.Execute(null, null, variables, CancellationToken.None);

            Transition[] input = variables["input"] as Transition[];
            Assert.NotNull(input);
            Assert.AreEqual(1, input.Length);
            Assert.AreEqual("\"Hello there\"", input[0].Log);
        }
        
        [Test, Parallelizable]
        public async Task OptionalParameter() {
            Mock<IScriptCompiler> compiler=new Mock<IScriptCompiler>();
            compiler.Setup(s => s.CompileCode(It.IsAny<string>(), ScriptLanguage.NCScript)).Returns<string, ScriptLanguage>((code, language) => new ScriptParser().Parse(code));
            compiler.Setup(s => s.CompileCodeAsync(It.IsAny<string>(), ScriptLanguage.NCScript)).Returns<string, ScriptLanguage>((code, language) => new ScriptParser().ParseAsync(code));
            
            StartNode node=new StartNode("Start", new StartParameters {
                Parameters = new[] {
                    new ParameterDeclaration {
                        Name = "input",
                        Type = "int",
                        Default = "12"
                    }
                }
            }, compiler.Object);

            Dictionary<string, object> variables = new Dictionary<string, object>();
            await node.Execute(null, null, variables, CancellationToken.None);

            Assert.That(variables.ContainsKey("input"));
            Assert.AreEqual(12, variables["input"]);
        }
    }
}