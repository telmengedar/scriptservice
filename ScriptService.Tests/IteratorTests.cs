using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NightlyCode.Database.Entities;
using NightlyCode.Scripting.Parser;
using NUnit.Framework;
using ScriptService.Dto.Tasks;
using ScriptService.Dto.Workflows;
using ScriptService.Services;
using ScriptService.Services.Cache;
using ScriptService.Services.Scripts;

namespace ScriptService.Tests {

    [TestFixture, Parallelizable]
    public class IteratorTests {

        [Test, Parallelizable]
        public async Task SumConditionalLoop() {
            IEntityManager database = TestSetup.CreateMemoryDatabase();
            CacheService cache = new CacheService(new NullLogger<CacheService>());
            WorkflowExecutionService executionservice = new WorkflowExecutionService(new NullLogger<WorkflowExecutionService>(), new Mock<IWorkflowService>().Object, new DatabaseTaskService(database), new ScriptCompiler(new NullLogger<ScriptCompiler>(), new ScriptParser(), cache, null), cache, new Mock<IScriptService>().Object, null);

            WorkableTask task = await executionservice.Execute(new WorkflowStructure {
                Name = "Test",
                Nodes = new[] {
                    new NodeData {
                        Type = NodeType.Start
                    },
                    new NodeData {
                        Type = NodeType.Value,
                        Parameters = new Dictionary<string, object> {
                            ["Value"]=0
                        },
                        Variable = "result"
                    },
                    new NodeData {
                        Type = NodeType.Iterator,
                        Parameters = new Dictionary<string, object> {
                            ["Collection"]="[1,5,2,2,8,7,4]"
                        }
                    },
                    new NodeData {
                        Type = NodeType.BinaryOperation,
                        Parameters = new Dictionary<string, object> {
                            ["lhs"]="$result",
                            ["rhs"]="$item",
                            ["operation"]="Add"
                        },
                        Variable = "result"
                    },
                    new NodeData {
                        Type = NodeType.Value,
                        Parameters = new Dictionary<string, object> {
                            ["Value"]="$result"
                        }
                    }
                },
                Transitions = new[] {
                    new IndexTransition {
                        OriginIndex = 0,
                        TargetIndex = 1,
                    },
                    new IndexTransition {
                        OriginIndex = 1,
                        TargetIndex = 2,
                    },
                    new IndexTransition {
                        OriginIndex = 2,
                        TargetIndex = 3,
                        Type = TransitionType.Loop,
                        Condition = "$item&1==0"
                    },
                    new IndexTransition {
                        OriginIndex = 2,
                        TargetIndex = 4,
                    },
                    new IndexTransition {
                        OriginIndex = 3,
                        TargetIndex = 2
                    }
                }
            });

            await task.Task;

            Assert.AreEqual(Dto.TaskStatus.Success, task.Status);
            Assert.AreEqual(16, task.Result);
        }

    }
}