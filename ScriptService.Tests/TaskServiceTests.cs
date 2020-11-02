using System;
using System.Threading.Tasks;
using NightlyCode.AspNetCore.Services.Data;
using NightlyCode.Database.Entities;
using NUnit.Framework;
using ScriptService.Dto.Tasks;
using ScriptService.Services;

namespace ScriptService.Tests {
    
    [TestFixture, Parallelizable]
    public class TaskServiceTests {

        [Test, Parallelizable]
        public async Task MostRecentFirst() {
            IEntityManager database = TestSetup.CreateMemoryDatabase();
            DatabaseTaskService taskservice = new DatabaseTaskService(database);

            for (int i = 0; i < 31; ++i) {
                await taskservice.StoreTask(new WorkableTask() {
                    Id = Guid.NewGuid(),
                    Started = new DateTime(2020, 01, 1 + i),
                    Finished = new DateTime(2020, 01, i + 1, 1, 0, 0)
                });
            }

            Page<WorkableTask> tasks = await taskservice.ListTasks(new TaskFilter {
                Count = 10,
            });

            Assert.AreEqual(10, tasks.Result.Length);
            Assert.AreEqual(31, tasks.Result[0].Started.Day);
        }
    }
}