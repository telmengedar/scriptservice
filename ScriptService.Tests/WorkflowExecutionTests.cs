using System.Collections.Generic;
using System.Threading.Tasks;
using Esprima.Ast;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NightlyCode.Database.Entities;
using NightlyCode.Scripting.Parser;
using NUnit.Framework;
using ScriptService.Dto;
using ScriptService.Dto.Tasks;
using ScriptService.Dto.Workflows;
using ScriptService.Services;
using ScriptService.Services.Cache;
using ScriptService.Services.JavaScript;
using ScriptService.Services.Scripts;
using ScriptService.Services.Workflows;
using ScriptService.Tests.Mocks;
using Utf8Json;
using Utf8Json.Resolvers;
using TaskStatus = ScriptService.Dto.TaskStatus;

namespace ScriptService.Tests {

    [TestFixture, Parallelizable]
    public class WorkflowExecutionTests {

        [Test, Parallelizable]
        public async Task SuspendAndContinue() {
            IEntityManager database = TestSetup.CreateMemoryDatabase();
            CacheService cache = new CacheService(new NullLogger<CacheService>());
            IScriptCompiler compiler = new ScriptCompiler(new NullLogger<ScriptCompiler>(), new ScriptParser(), cache, null, new Mock<IScriptService>().Object, new Mock<IArchiveService>().Object, null);
            WorkflowExecutionService executionservice = new WorkflowExecutionService(new NullLogger<WorkflowExecutionService>(), new DatabaseTaskService(database), null);
            WorkflowCompiler workflowcompiler = new WorkflowCompiler(new NullLogger<WorkflowCompiler>(), cache, null, compiler, executionservice);

            WorkableTask task = await executionservice.Execute(await workflowcompiler.BuildWorkflow(new WorkflowStructure {
                Name = "Test",
                Nodes = new [] {
                    new NodeData {
                        Type = NodeType.Start
                    },
                    new NodeData {
                        Type = NodeType.Suspend,
                        Parameters = new Dictionary<string, object> {
                            ["Variable"]="x"
                        }
                    },
                    new NodeData {
                        Type = NodeType.Value,
                        Parameters = new Dictionary<string, object> {
                            ["Value"]=10
                        }
                    },
                    new NodeData {
                        Type = NodeType.Value,
                        Parameters = new Dictionary<string, object> {
                            ["Value"]=5
                        }
                    }
                },
                Transitions = new [] {
                    new IndexTransition {
                        OriginIndex = 0,
                        TargetIndex = 1,
                    },
                    new IndexTransition {
                        OriginIndex = 1,
                        TargetIndex = 2,
                        Condition = "$x>=3"
                    },
                    new IndexTransition {
                        OriginIndex = 1,
                        TargetIndex = 3,
                    }
                }
            }));

            await task.Task;

            Assert.AreEqual(TaskStatus.Suspended, task.Status);

            task = await executionservice.Continue(task.Id);

            await task.Task;

            Assert.AreEqual(TaskStatus.Success, task.Status);
            Assert.AreEqual(5, task.Result);
        }

        [Test, Parallelizable]
        public async Task ExecuteJavascriptExpression() {
            Mock<IJavascriptImportService> importservice=new Mock<IJavascriptImportService>();
            importservice.Setup(s => s.Clone(It.IsAny<WorkableLogger>())).Returns(() => importservice.Object);

            IEntityManager database = TestSetup.CreateMemoryDatabase();
            CacheService cache = new CacheService(new NullLogger<CacheService>());
            IScriptCompiler compiler = new ScriptCompiler(new NullLogger<ScriptCompiler>(), new ScriptParser(), cache, null, new Mock<IScriptService>().Object, new Mock<IArchiveService>().Object, importservice.Object);
            WorkflowExecutionService executionservice = new WorkflowExecutionService(new NullLogger<WorkflowExecutionService>(), new DatabaseTaskService(database), null);
            WorkflowCompiler workflowcompiler = new WorkflowCompiler(new NullLogger<WorkflowCompiler>(), cache, null, compiler, executionservice);

            WorkableTask task = await executionservice.Execute(await workflowcompiler.BuildWorkflow(new WorkflowStructure {
                Name = "JS Test",
                Nodes = new[] {
                    new NodeData {
                        Type = NodeType.Start
                    },
                    new NodeData {
                        Type = NodeType.Expression,
                        Parameters = new Dictionary<string, object> {
                            ["Code"] = "return a+b",
                            ["Language"] = ScriptLanguage.JavaScript
                        }
                    }
                },
                Transitions = new[] {
                    new IndexTransition {
                        OriginIndex = 0,
                        TargetIndex = 1
                    },
                }
            }, new Dictionary<string, object> {
                ["a"] = 3,
                ["b"] = 4
            }));

            await task.Task;
            Assert.AreEqual(TaskStatus.Success, task.Status);
            Assert.AreEqual(7, task.Result);
        }

        [Test, Parallelizable]
        public async Task ExecuteSubWorkflowWithParameters() {
            Mock<IJavascriptImportService> importservice=new Mock<IJavascriptImportService>();
            importservice.Setup(s => s.Clone(It.IsAny<WorkableLogger>())).Returns(() => importservice.Object);
            Mock<IArchiveService> archiveservice=new Mock<IArchiveService>();
            
            IEntityManager database = TestSetup.CreateMemoryDatabase();
            CacheService cache = new CacheService(new NullLogger<CacheService>());
            IScriptCompiler compiler = new ScriptCompiler(new NullLogger<ScriptCompiler>(), new ScriptParser(), cache, null, new Mock<IScriptService>().Object, archiveservice.Object, importservice.Object);
            WorkflowExecutionService executionservice = new WorkflowExecutionService(new NullLogger<WorkflowExecutionService>(), new DatabaseTaskService(database), null);
            IWorkflowService workflowservice = new DatabaseWorkflowService(database, archiveservice.Object);
            WorkflowCompiler workflowcompiler = new WorkflowCompiler(new NullLogger<WorkflowCompiler>(), cache, workflowservice, compiler, executionservice);

            await workflowservice.CreateWorkflow(new WorkflowStructure {
                Name = "Submarine",
                Nodes=new [] {
                    new NodeData {
                        Type = NodeType.Start
                    },
                    new NodeData {
                        Type = NodeType.Value,
                        Parameters = new Dictionary<string, object> {
                            ["Value"]="$job.id"
                        }
                    }
                },
                Transitions = new[] {
                    new IndexTransition {
                        OriginIndex = 0,
                        TargetIndex = 1
                    },
                }
            });

            WorkableTask task = await executionservice.Execute(await workflowcompiler.BuildWorkflow(new WorkflowStructure {
                Name = "Subcall Test",
                Nodes = new[] {
                    new NodeData {
                        Type = NodeType.Start
                    },
                    new NodeData {
                        Type = NodeType.Workflow,
                        Parameters = new Dictionary<string, object> {
                            ["Name"] = "Submarine",
                            ["Arguments"] = new Dictionary<string, object> {
                                ["job"] = new Dictionary<string, object> {
                                    ["id"] = 1.0
                                }
                            }
                        }
                    }
                },
                Transitions = new[] {
                    new IndexTransition {
                        OriginIndex = 0,
                        TargetIndex = 1
                    },
                }
            }));

            await task.Task;
            Assert.AreEqual(TaskStatus.Success, task.Status);
            Assert.AreEqual(1.0, task.Result);
        }
        
        [Test, Parallelizable]
        public async Task SuspendAndContinueWithParameters() {
            IEntityManager database = TestSetup.CreateMemoryDatabase();
            CacheService cache = new CacheService(new NullLogger<CacheService>());
            IScriptCompiler compiler = new ScriptCompiler(new NullLogger<ScriptCompiler>(), new ScriptParser(), cache, null, new Mock<IScriptService>().Object, new Mock<IArchiveService>().Object, null);
            WorkflowExecutionService executionservice = new WorkflowExecutionService(new NullLogger<WorkflowExecutionService>(), new DatabaseTaskService(database), null);
            WorkflowCompiler workflowcompiler = new WorkflowCompiler(new NullLogger<WorkflowCompiler>(), cache, null, compiler, executionservice);

            WorkableTask task = await executionservice.Execute(await workflowcompiler.BuildWorkflow(new WorkflowStructure {
                Name = "Test",
                Nodes = new[] {
                    new NodeData {
                        Type = NodeType.Start
                    },
                    new NodeData {
                        Type = NodeType.Suspend,
                        Parameters = new Dictionary<string, object> {
                            ["Variable"]="x"
                        }
                    },
                    new NodeData {
                        Type = NodeType.Value,
                        Parameters = new Dictionary<string, object> {
                            ["Value"]=10
                        }
                    },
                    new NodeData {
                        Type = NodeType.Value,
                        Parameters = new Dictionary<string, object> {
                            ["Value"]=5
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
                        Condition = "$x>=3"
                    },
                    new IndexTransition {
                        OriginIndex = 1,
                        TargetIndex = 3,
                    }
                }
            }));

            await task.Task;

            Assert.AreEqual(TaskStatus.Suspended, task.Status);

            task = await executionservice.Continue(task.Id, new Dictionary<string, object> {
                ["x"] = 5
            });

            await task.Task;

            Assert.AreEqual(TaskStatus.Success, task.Status);
            Assert.AreEqual(10, task.Result);
        }

    }
}