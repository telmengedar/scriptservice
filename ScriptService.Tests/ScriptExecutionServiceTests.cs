using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using ScriptService.Dto;
using ScriptService.Dto.Tasks;
using ScriptService.Services;
using ScriptService.Services.Scripts;
using ScriptService.Tests.Mocks;
using TaskStatus = ScriptService.Dto.TaskStatus;

namespace ScriptService.Tests {

    [TestFixture, Parallelizable]
    public class ScriptExecutionServiceTests {

        [Test, Parallelizable]
        public async Task ExecuteWithDictionary() {
            Mock<ITaskService> taskservice=new Mock<ITaskService>();
            taskservice.Setup(s => s.CreateTask(WorkableType.Script, 0, 0, "Test", It.IsAny<IDictionary<string, object>>())).Returns(new WorkableTask() {
                Token = new CancellationTokenSource(),
                Log = new List<string>(),
                Status = TaskStatus.Running
            });

            ScriptExecutionService service = new ScriptExecutionService(new NullLogger<ScriptExecutionService>(), taskservice.Object, new TestCompiler());
            WorkableTask task = await service.Execute(new NamedCode {
                Name = "Test",
                Code = "return(param.property)"
            }, new Dictionary<string, object> {
                ["param"] = "{\"property\":3}"
            }, TimeSpan.FromSeconds(10));

            Assert.AreEqual(TaskStatus.Success, task.Status);
            Assert.AreEqual(3, task.Result);
        }
    }
}