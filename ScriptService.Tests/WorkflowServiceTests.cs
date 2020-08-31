using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NightlyCode.Database.Entities;
using NUnit.Framework;
using ScriptService.Dto.Workflows;
using ScriptService.Extensions;
using ScriptService.Services;

namespace ScriptService.Tests {

    [TestFixture, Parallelizable]
    public class WorkflowServiceTests {

        [Test, Parallelizable]
        public void ObjectDeserializeOutcome() {
            string json = "{\"id\": \"guid\", \"name\": \"bla\", \"type\": \"Start\"}";
            object result = json.Deserialize<object>();
            Assert.That(result is IDictionary<string, object>);
        }

        [Test, Parallelizable]
        public void ArrayDeserializeOutcome() {
            string json = "[{\"id\": \"guid\", \"name\": \"bla\", \"type\": \"Start\"}]";
            object result = json.Deserialize<object>();
            Assert.That(result is List<object> array);
            array = (List<object>) result; // what a hack ...
            Assert.AreEqual(1, array.Count);
            Assert.That(array[0] is IDictionary<string, object>);
        }

        [Test, Parallelizable]
        public async Task GetWorkflowByName() {
            IEntityManager database = TestSetup.CreateMemoryDatabase();
            IWorkflowService service = new DatabaseWorkflowService(database, new Mock<IArchiveService>().Object);

            await service.CreateWorkflow(new WorkflowStructure {
                Name = "Test"
            });
            await service.CreateWorkflow(new WorkflowStructure {
                Name = "Plum"
            });
            await service.CreateWorkflow(new WorkflowStructure {
                Name = "Sollbestand"
            });

            WorkflowDetails workflow = await service.GetWorkflow("Plum");
            Assert.NotNull(workflow);
            Assert.AreEqual("Plum", workflow.Name);
        }
    }
}