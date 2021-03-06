﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NightlyCode.AspNetCore.Services.Data;
using NightlyCode.Database.Entities;
using NightlyCode.Scripting.Parser;
using NUnit.Framework;
using ScriptService.Dto;
using ScriptService.Dto.Tasks;
using ScriptService.Dto.Workflows;
using ScriptService.Dto.Workflows.Nodes;
using ScriptService.Services;
using ScriptService.Services.Cache;
using ScriptService.Services.JavaScript;
using ScriptService.Services.Scripts;
using ScriptService.Services.Workflows;
using TaskStatus = ScriptService.Dto.TaskStatus;

namespace ScriptService.Tests {

    [TestFixture, Parallelizable]
    public class WorkflowExecutionTests {

        [Test, Parallelizable]
        public async Task SuspendAndContinue() {
            IEntityManager database = TestSetup.CreateMemoryDatabase();
            CacheService cache = new CacheService(new NullLogger<CacheService>());
            IScriptCompiler compiler = new ScriptCompiler(new NullLogger<ScriptCompiler>(), new ScriptParser(), cache, null, new Mock<IScriptService>().Object, new Mock<IArchiveService>().Object, null, null, null);
            WorkflowCompiler workflowcompiler = new WorkflowCompiler(new NullLogger<WorkflowCompiler>(), cache, null, compiler);
            WorkflowExecutionService executionservice = new WorkflowExecutionService(new NullLogger<WorkflowExecutionService>(), new DatabaseTaskService(database), null, null, workflowcompiler);

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
            }), new Dictionary<string, object>(), false);

            await task.Task;

            Assert.AreEqual(TaskStatus.Suspended, task.Status);

            task = await executionservice.Continue(task.Id);

            await task.Task;

            Assert.AreEqual(TaskStatus.Success, task.Status);
            Assert.AreEqual(5, task.Result);
        }

        [Test, Parallelizable]
        public async Task ExecuteJavascriptExpression() {
            Mock<IScriptImportService> importservice=new Mock<IScriptImportService>();
            importservice.Setup(s => s.Clone(It.IsAny<WorkableLogger>())).Returns(() => importservice.Object);

            IEntityManager database = TestSetup.CreateMemoryDatabase();
            CacheService cache = new CacheService(new NullLogger<CacheService>());
            IScriptCompiler compiler = new ScriptCompiler(new NullLogger<ScriptCompiler>(), new ScriptParser(), cache, null, new Mock<IScriptService>().Object, new Mock<IArchiveService>().Object, importservice.Object, null, null);
            WorkflowCompiler workflowcompiler = new WorkflowCompiler(new NullLogger<WorkflowCompiler>(), cache, null, compiler);
            WorkflowExecutionService executionservice = new WorkflowExecutionService(new NullLogger<WorkflowExecutionService>(), new DatabaseTaskService(database), null, null, workflowcompiler);

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
            }), new Dictionary<string, object> {
                ["a"] = 3,
                ["b"] = 4
            }, false);

            await task.Task;
            Assert.AreEqual(TaskStatus.Success, task.Status);
            Assert.AreEqual(7, task.Result);
        }

        [Test, Parallelizable]
        [Timeout(5000)]
        public async Task ExecuteLoop() {
            Mock<IScriptImportService> importservice=new Mock<IScriptImportService>();
            importservice.Setup(s => s.Clone(It.IsAny<WorkableLogger>())).Returns(() => importservice.Object);

            IEntityManager database = TestSetup.CreateMemoryDatabase();
            CacheService cache = new CacheService(new NullLogger<CacheService>());
            IScriptCompiler compiler = new ScriptCompiler(new NullLogger<ScriptCompiler>(), new ScriptParser(), cache, null, new Mock<IScriptService>().Object, new Mock<IArchiveService>().Object, importservice.Object, null, null);
            WorkflowCompiler workflowcompiler = new WorkflowCompiler(new NullLogger<WorkflowCompiler>(), cache, null, compiler);
            WorkflowExecutionService executionservice = new WorkflowExecutionService(new NullLogger<WorkflowExecutionService>(), new DatabaseTaskService(database), null, null, workflowcompiler);

            WorkableTask task = await executionservice.Execute(await workflowcompiler.BuildWorkflow(new WorkflowStructure {
                Name = "JS Test",
                Nodes = new[] {
                    new NodeData {
                        Type = NodeType.Start
                    },
                    new NodeData {
                        Type = NodeType.Value,
                        Parameters = new Dictionary<string, object> {
                            ["Value"] = "0"
                        },
                        Variable = "result"
                    },
                    new NodeData {
                        Type = NodeType.Iterator,
                        Parameters = new Dictionary<string, object> {
                            ["Collection"] = "[1,2,3,4,5]",
                            ["Item"] = "number"
                        },
                    },
                    new NodeData {
                        Type = NodeType.BinaryOperation,
                        Parameters = new Dictionary<string, object> {
                            ["Lhs"] = "$result",
                            ["Rhs"]="$number",
                            ["Op"]=BinaryOperation.Add
                        },
                        Variable = "result"
                    },
                    new NodeData {
                        Type = NodeType.Value,
                        Parameters = new Dictionary<string, object> {
                            ["Value"] = "$result"
                        }
                    }
                },
                Transitions = new[] {
                    new IndexTransition {
                        OriginIndex = 0,
                        TargetIndex = 1
                    },
                    new IndexTransition {
                        OriginIndex = 1,
                        TargetIndex = 2,
                    },
                    new IndexTransition {
                        OriginIndex = 2,
                        TargetIndex = 3,
                        Type = TransitionType.Loop,
                        Log = "$\"Adding number {$number}\""
                    },
                    new IndexTransition {
                        OriginIndex = 3,
                        TargetIndex = 2
                    },
                    new IndexTransition {
                        OriginIndex = 2,
                        TargetIndex = 4
                    },
                }
            }), new Dictionary<string, object>(), false);

            await task.Task;
            Assert.AreEqual(TaskStatus.Success, task.Status);
            Assert.AreEqual(15, task.Result);
        }

        [Test, Parallelizable]
        [Timeout(5000)]
        public async Task ExecuteJSLoop() {
            Mock<IScriptImportService> importservice=new Mock<IScriptImportService>();
            importservice.Setup(s => s.Clone(It.IsAny<WorkableLogger>())).Returns(() => importservice.Object);

            IEntityManager database = TestSetup.CreateMemoryDatabase();
            CacheService cache = new CacheService(new NullLogger<CacheService>());
            IScriptCompiler compiler = new ScriptCompiler(new NullLogger<ScriptCompiler>(), new ScriptParser(), cache, null, new Mock<IScriptService>().Object, new Mock<IArchiveService>().Object, importservice.Object, null, null);
            WorkflowCompiler workflowcompiler = new WorkflowCompiler(new NullLogger<WorkflowCompiler>(), cache, null, compiler);
            WorkflowExecutionService executionservice = new WorkflowExecutionService(new NullLogger<WorkflowExecutionService>(), new DatabaseTaskService(database), null, null, workflowcompiler);

            WorkableTask task = await executionservice.Execute(await workflowcompiler.BuildWorkflow(new WorkflowStructure {
                Name = "Typescript Workflow Test",
                Language = ScriptLanguage.JavaScript,
                Nodes = new[] {
                    new NodeData {
                        Type = NodeType.Start
                    },
                    new NodeData {
                        Type = NodeType.Value,
                        Parameters = new Dictionary<string, object> {
                            ["Value"] = "0"
                        },
                        Variable = "result"
                    },
                    new NodeData {
                        Type = NodeType.Iterator,
                        Parameters = new Dictionary<string, object> {
                            ["Collection"] = "[1,2,3,4,5]",
                            ["Item"] = "number"
                        },
                    },
                    new NodeData {
                        Type = NodeType.BinaryOperation,
                        Parameters = new Dictionary<string, object> {
                            ["Lhs"] = "result",
                            ["Rhs"]="number",
                            ["Op"]=BinaryOperation.Add
                        },
                        Variable = "result"
                    },
                    new NodeData {
                        Type = NodeType.Value,
                        Parameters = new Dictionary<string, object> {
                            ["Value"] = "result"
                        }
                    }
                },
                Transitions = new[] {
                    new IndexTransition {
                        OriginIndex = 0,
                        TargetIndex = 1
                    },
                    new IndexTransition {
                        OriginIndex = 1,
                        TargetIndex = 2,
                    },
                    new IndexTransition {
                        OriginIndex = 2,
                        TargetIndex = 3,
                        Type = TransitionType.Loop,
                        Log = "`Adding number ${number}`"
                    },
                    new IndexTransition {
                        OriginIndex = 3,
                        TargetIndex = 2
                    },
                    new IndexTransition {
                        OriginIndex = 2,
                        TargetIndex = 4
                    },
                }
            }), new Dictionary<string, object>(), false);

            await task.Task;
            Assert.AreEqual(TaskStatus.Success, task.Status);
            Assert.AreEqual(15, task.Result);
        }

        [Test, Parallelizable]
        public async Task ExecuteSubWorkflowWithParameters() {
            Mock<IScriptImportService> importservice=new Mock<IScriptImportService>();
            importservice.Setup(s => s.Clone(It.IsAny<WorkableLogger>())).Returns(() => importservice.Object);
            Mock<IArchiveService> archiveservice=new Mock<IArchiveService>();
            
            IEntityManager database = TestSetup.CreateMemoryDatabase();
            CacheService cache = new CacheService(new NullLogger<CacheService>());
            IScriptCompiler compiler = new ScriptCompiler(new NullLogger<ScriptCompiler>(), new ScriptParser(), cache, null, new Mock<IScriptService>().Object, archiveservice.Object, importservice.Object, null, null);
            IWorkflowService workflowservice = new DatabaseWorkflowService(database, archiveservice.Object);
            WorkflowCompiler workflowcompiler = new WorkflowCompiler(new NullLogger<WorkflowCompiler>(), cache, workflowservice, compiler);
            WorkflowExecutionService executionservice = new WorkflowExecutionService(new NullLogger<WorkflowExecutionService>(), new DatabaseTaskService(database), null, workflowservice, workflowcompiler);

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
            }), new Dictionary<string, object>(), false);

            await task.Task;
            Assert.AreEqual(TaskStatus.Success, task.Status);
            Assert.AreEqual(1.0, task.Result);
        }

        [Test, Parallelizable]
        public async Task ProfileSubWorkflow() {
            Mock<IScriptImportService> importservice=new Mock<IScriptImportService>();
            importservice.Setup(s => s.Clone(It.IsAny<WorkableLogger>())).Returns(() => importservice.Object);
            Mock<IArchiveService> archiveservice=new Mock<IArchiveService>();
            
            IEntityManager database = TestSetup.CreateMemoryDatabase();
            CacheService cache = new CacheService(new NullLogger<CacheService>());
            IScriptCompiler compiler = new ScriptCompiler(new NullLogger<ScriptCompiler>(), new ScriptParser(), cache, null, new Mock<IScriptService>().Object, archiveservice.Object, importservice.Object, null, null);
            IWorkflowService workflowservice = new DatabaseWorkflowService(database, archiveservice.Object);
            WorkflowCompiler workflowcompiler = new WorkflowCompiler(new NullLogger<WorkflowCompiler>(), cache, workflowservice, compiler);
            WorkflowExecutionService executionservice = new WorkflowExecutionService(new NullLogger<WorkflowExecutionService>(), new DatabaseTaskService(database), null, workflowservice, workflowcompiler);

            await workflowservice.CreateWorkflow(new WorkflowStructure {
                Name = "Submarine",
                Nodes=new [] {
                    new NodeData {
                        Type = NodeType.Start
                    },
                    new NodeData {
                        Type = NodeType.Value,
                        Name = "Subvalue",
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
                        Name = "Call Submarine",
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
            }), new Dictionary<string, object>(), true);

            await task.Task;
            Assert.AreEqual(TaskStatus.Success, task.Status);
            Assert.That(task.Log.Any(l=>l.Contains("calls")));
            Assert.AreEqual(1.0, task.Result);
        }

        [Test, Parallelizable]
        public async Task SuspendAndContinueWithParameters() {
            IEntityManager database = TestSetup.CreateMemoryDatabase();
            CacheService cache = new CacheService(new NullLogger<CacheService>());
            IScriptCompiler compiler = new ScriptCompiler(new NullLogger<ScriptCompiler>(), new ScriptParser(), cache, null, new Mock<IScriptService>().Object, new Mock<IArchiveService>().Object, null, null, null);
            WorkflowCompiler workflowcompiler = new WorkflowCompiler(new NullLogger<WorkflowCompiler>(), cache, null, compiler);
            WorkflowExecutionService executionservice = new WorkflowExecutionService(new NullLogger<WorkflowExecutionService>(), new DatabaseTaskService(database), null, null, workflowcompiler);

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
            }), new Dictionary<string, object>(), false);

            await task.Task;

            Assert.AreEqual(TaskStatus.Suspended, task.Status);

            task = await executionservice.Continue(task.Id, new Dictionary<string, object> {
                ["x"] = 5
            });

            await task.Task;

            Assert.AreEqual(TaskStatus.Success, task.Status);
            Assert.AreEqual(10, task.Result);
        }

        [Test, Parallelizable]
        [Timeout(5000)]
        public async Task ExecuteSubWorkflowWithParametersInLoop() {
            Mock<IScriptImportService> importservice=new Mock<IScriptImportService>();
            importservice.Setup(s => s.Clone(It.IsAny<WorkableLogger>())).Returns(() => importservice.Object);
            Mock<IArchiveService> archiveservice=new Mock<IArchiveService>();
            
            IEntityManager database = TestSetup.CreateMemoryDatabase();
            CacheService cache = new CacheService(new NullLogger<CacheService>());
            IScriptCompiler compiler = new ScriptCompiler(new NullLogger<ScriptCompiler>(), new ScriptParser(), cache, null, new Mock<IScriptService>().Object, archiveservice.Object, importservice.Object, null, null);
            IWorkflowService workflowservice = new DatabaseWorkflowService(database, archiveservice.Object);
            WorkflowCompiler workflowcompiler = new WorkflowCompiler(new NullLogger<WorkflowCompiler>(), cache, workflowservice, compiler);
            WorkflowExecutionService executionservice = new WorkflowExecutionService(new NullLogger<WorkflowExecutionService>(), new DatabaseTaskService(database), null, workflowservice, workflowcompiler);

            await workflowservice.CreateWorkflow(new WorkflowStructure {
                Name = "Submarine",
                Nodes=new [] {
                    new NodeData {
                        Type = NodeType.Start
                    },
                    new NodeData {
                        Type = NodeType.Value,
                        Parameters = new Dictionary<string, object> {
                            ["Value"]="$value"
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
                        Type = NodeType.Value,
                        Parameters = new Dictionary<string, object> {
                            ["Value"]="0",
                        },
                        Variable = "result" 
                    },
                    new NodeData {
                        Type = NodeType.Iterator,
                        Parameters = new Dictionary<string, object> {
                            ["Item"]="value",
                            ["Collection"]="[1,2,3,4,5]"
                        }
                    },
                    new NodeData {
                        Type = NodeType.Workflow,
                        Parameters = new Dictionary<string, object> {
                            ["Name"] = "Submarine",
                            ["Arguments"] = new Dictionary<string, object> {
                                ["value"] = "$value"
                            }
                        },
                        Variable = "subresult"
                    },
                    new NodeData {
                        Type=NodeType.BinaryOperation,
                        Parameters = new Dictionary<string, object> {
                            ["Lhs"]="$result",
                            ["Operation"]=BinaryOperation.Add,
                            ["Rhs"]="$subresult"
                        },
                        Variable = "result"
                    },
                    new NodeData {
                        Type = NodeType.Value,
                        Parameters = new Dictionary<string, object> {
                            ["Value"]="$result",
                        }
                    },

                },
                Transitions = new[] {
                    new IndexTransition {
                        OriginIndex = 0,
                        TargetIndex = 1
                    },
                    new IndexTransition {
                        OriginIndex = 1,
                        TargetIndex = 2
                    },
                    new IndexTransition {
                        OriginIndex = 2,
                        TargetIndex = 3,
                        Type = TransitionType.Loop
                    },
                    new IndexTransition {
                        OriginIndex = 2,
                        TargetIndex = 5
                    },
                    new IndexTransition {
                        OriginIndex = 3,
                        TargetIndex = 4
                    },
                    new IndexTransition {
                        OriginIndex = 4,
                        TargetIndex = 2
                    },
                }
            }), new Dictionary<string, object>(), false);

            await task.Task;
            Assert.AreEqual(TaskStatus.Success, task.Status);
            Assert.AreEqual(15, task.Result);
        }

        [Test, Parallelizable]
        [Timeout(5000)]
        public async Task AbortInfiniteLoop() {
            Mock<IScriptImportService> importservice=new Mock<IScriptImportService>();
            importservice.Setup(s => s.Clone(It.IsAny<WorkableLogger>())).Returns(() => importservice.Object);

            IEntityManager database = TestSetup.CreateMemoryDatabase();
            CacheService cache = new CacheService(new NullLogger<CacheService>());
            IScriptCompiler compiler = new ScriptCompiler(new NullLogger<ScriptCompiler>(), new ScriptParser(), cache, null, new Mock<IScriptService>().Object, new Mock<IArchiveService>().Object, importservice.Object, null, null);
            WorkflowCompiler workflowcompiler = new WorkflowCompiler(new NullLogger<WorkflowCompiler>(), cache, null, compiler);
            WorkflowExecutionService executionservice = new WorkflowExecutionService(new NullLogger<WorkflowExecutionService>(), new DatabaseTaskService(database), null, null, workflowcompiler);

            WorkableTask task = await executionservice.Execute(await workflowcompiler.BuildWorkflow(new WorkflowStructure {
                Name = "JS Test",
                Nodes = new[] {
                    new NodeData {
                        Type = NodeType.Start
                    },
                    new NodeData {
                        Type = NodeType.Node,
                    }
                },
                Transitions = new[] {
                    new IndexTransition {
                        OriginIndex = 0,
                        TargetIndex = 1
                    },
                    new IndexTransition {
                        OriginIndex = 1,
                        TargetIndex = 0,
                    }
                }
            }), new Dictionary<string, object>(), false);

            Assert.AreEqual(TaskStatus.Running, task.Status);
            task.Token.Cancel();
            await Task.WhenAny(task.Task, Task.Delay(1000));
            Assert.AreEqual(TaskStatus.Canceled, task.Status);
        }
        
        [Test, Parallelizable]
        [Timeout(5000)]
        public async Task ListRunningTask() {
            Mock<IScriptImportService> importservice=new Mock<IScriptImportService>();
            importservice.Setup(s => s.Clone(It.IsAny<WorkableLogger>())).Returns(() => importservice.Object);

            IEntityManager database = TestSetup.CreateMemoryDatabase();
            CacheService cache = new CacheService(new NullLogger<CacheService>());
            IScriptCompiler compiler = new ScriptCompiler(new NullLogger<ScriptCompiler>(), new ScriptParser(), cache, null, new Mock<IScriptService>().Object, new Mock<IArchiveService>().Object, importservice.Object, null, null);
            WorkflowCompiler workflowcompiler = new WorkflowCompiler(new NullLogger<WorkflowCompiler>(), cache, null, compiler);
            ITaskService taskservice = new DatabaseTaskService(database);
            WorkflowExecutionService executionservice = new WorkflowExecutionService(new NullLogger<WorkflowExecutionService>(), taskservice, null, null, workflowcompiler);

            WorkableTask task = await executionservice.Execute(await workflowcompiler.BuildWorkflow(new WorkflowStructure {
                Name = "JS Test",
                Nodes = new[] {
                    new NodeData {
                        Type = NodeType.Start
                    },
                    new NodeData {
                        Type = NodeType.Node,
                    }
                },
                Transitions = new[] {
                    new IndexTransition {
                        OriginIndex = 0,
                        TargetIndex = 1
                    },
                    new IndexTransition {
                        OriginIndex = 1,
                        TargetIndex = 0,
                    }
                }
            }), new Dictionary<string, object>(), false);

            Assert.AreEqual(TaskStatus.Running, task.Status);
            Page<WorkableTask> tasks=await taskservice.ListTasks();
            Assert.AreEqual(1, tasks.Total);
            Assert.AreEqual(1, tasks.Result.Length);
            
            task.Token.Cancel();
            await Task.WhenAny(task.Task, Task.Delay(1000));
            Assert.AreEqual(TaskStatus.Canceled, task.Status);
        }

        [Test, Parallelizable]
        [Timeout(5000)]
        public async Task ProfileLoopPerformance() {
            Mock<IScriptImportService> importservice=new Mock<IScriptImportService>();
            importservice.Setup(s => s.Clone(It.IsAny<WorkableLogger>())).Returns(() => importservice.Object);

            IEntityManager database = TestSetup.CreateMemoryDatabase();
            CacheService cache = new CacheService(new NullLogger<CacheService>());
            IScriptCompiler compiler = new ScriptCompiler(new NullLogger<ScriptCompiler>(), new ScriptParser(), cache, null, new Mock<IScriptService>().Object, new Mock<IArchiveService>().Object, importservice.Object, null, null);
            WorkflowCompiler workflowcompiler = new WorkflowCompiler(new NullLogger<WorkflowCompiler>(), cache, null, compiler);
            WorkflowExecutionService executionservice = new WorkflowExecutionService(new NullLogger<WorkflowExecutionService>(), new DatabaseTaskService(database), null, null, workflowcompiler);

            WorkableTask task = await executionservice.Execute(await workflowcompiler.BuildWorkflow(new WorkflowStructure {
                Name = "JS Test",
                Nodes = new[] {
                    new NodeData {
                        Type = NodeType.Start
                    },
                    new NodeData {
                        Type = NodeType.Value,
                        Parameters = new Dictionary<string, object> {
                            ["Value"] = "0"
                        },
                        Variable = "result"
                    },
                    new NodeData {
                        Type = NodeType.Iterator,
                        Parameters = new Dictionary<string, object> {
                            ["Collection"] = "[1,2,3,4,5]",
                            ["Item"] = "number"
                        },
                    },
                    new NodeData {
                        Type = NodeType.BinaryOperation,
                        Parameters = new Dictionary<string, object> {
                            ["Lhs"] = "$result",
                            ["Rhs"]="$number",
                            ["Op"]=BinaryOperation.Add
                        },
                        Variable = "result"
                    },
                    new NodeData {
                        Type = NodeType.Value,
                        Parameters = new Dictionary<string, object> {
                            ["Value"] = "$result"
                        }
                    }
                },
                Transitions = new[] {
                    new IndexTransition {
                        OriginIndex = 0,
                        TargetIndex = 1
                    },
                    new IndexTransition {
                        OriginIndex = 1,
                        TargetIndex = 2,
                    },
                    new IndexTransition {
                        OriginIndex = 2,
                        TargetIndex = 3,
                        Type = TransitionType.Loop,
                        Log = "$\"Adding number {$number}\""
                    },
                    new IndexTransition {
                        OriginIndex = 3,
                        TargetIndex = 2
                    },
                    new IndexTransition {
                        OriginIndex = 2,
                        TargetIndex = 4
                    },
                }
            }), new Dictionary<string, object>(), true);

            await task.Task;
            Assert.AreEqual(TaskStatus.Success, task.Status);
            Assert.That(task.Log.Any(l=>l.Contains("calls")));
            Assert.AreEqual(15, task.Result);
        }

    }
}