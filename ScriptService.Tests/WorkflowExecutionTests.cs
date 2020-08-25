﻿using System.Collections.Generic;
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
using TaskStatus = ScriptService.Dto.TaskStatus;

namespace ScriptService.Tests {

    [TestFixture, Parallelizable]
    public class WorkflowExecutionTests {

        [Test, Parallelizable]
        public async Task SuspendAndContinue() {
            IEntityManager database = TestSetup.CreateMemoryDatabase();
            CacheService cache = new CacheService(new NullLogger<CacheService>());
            WorkflowExecutionService executionservice = new WorkflowExecutionService(new NullLogger<WorkflowExecutionService>(), new Mock<IWorkflowService>().Object, new DatabaseTaskService(database), new ScriptCompiler(new NullLogger<ScriptCompiler>(), new ScriptParser(), cache, null), cache, new Mock<IScriptService>().Object, null);

            WorkableTask task = await executionservice.Execute(new WorkflowStructure {
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
            });

            await task.Task;

            Assert.AreEqual(TaskStatus.Suspended, task.Status);

            task = await executionservice.Continue(task.Id);

            await task.Task;

            Assert.AreEqual(TaskStatus.Success, task.Status);
            Assert.AreEqual(5, task.Result);
        }

        [Test, Parallelizable]
        public async Task SuspendAndContinueWidthParameters() {
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
            });

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