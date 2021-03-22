using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NightlyCode.AspNetCore.Services.Data;
using NightlyCode.AspNetCore.Services.Errors.Exceptions;
using NightlyCode.Database.Entities;
using NightlyCode.Database.Entities.Operations;
using NightlyCode.Database.Entities.Operations.Prepared;
using NightlyCode.Database.Expressions;
using NightlyCode.Database.Fields;
using NightlyCode.Database.Tokens;
using ScriptService.Dto;
using ScriptService.Dto.Tasks;
using ScriptService.Extensions;
using TaskStatus = ScriptService.Dto.TaskStatus;

namespace ScriptService.Services {

    /// <inheritdoc />
    public class DatabaseTaskService : ITaskService {
        readonly IEntityManager database;
        readonly ConcurrentDictionary<Guid, WorkableTask> runningtasks=new ConcurrentDictionary<Guid, WorkableTask>();

        readonly PreparedOperation insert;
        readonly PreparedLoadOperation<TaskDb> loadtask;

        /// <summary>
        /// creates a new <see cref="DatabaseTaskService"/>
        /// </summary>
        /// <param name="database">access to database</param>
        public DatabaseTaskService(IEntityManager database) {
            this.database = database;
            database.UpdateSchema<TaskDb>();
            insert = database.Insert<TaskDb>().Columns(t => t.Id, t=>t.Type, t => t.WorkableId, t => t.WorkableRevision, t => t.WorkableName, t => t.Parameters, t => t.Started, t => t.Finished, t => t.Status, t => t.Result, t => t.Log, t=>t.Performance).Prepare();
            loadtask = database.Load<TaskDb>().Where(t => t.Id == DBParameter.Guid).Prepare();
        }

        /// <inheritdoc />
        public WorkableTask CreateTask(WorkableType type, long workableid, int workablerevision, string workablename, IDictionary<string, object> variables) {
            WorkableTask task = new WorkableTask {
                Id = Guid.NewGuid(),
                Type = type,
                WorkableId = workableid,
                WorkableRevision = workablerevision,
                WorkableName = workablename,
                Log = new List<string>(),
                Performance = new List<ProfilingEntry>(),
                Started = DateTime.Now,
                Parameters = variables,
                Status = TaskStatus.Running,
                Token = new CancellationTokenSource()
            };

            runningtasks[task.Id] = task;
            return task;
        }

        /// <inheritdoc />
        public Task StoreTask(WorkableTask task) {
            task.Finished ??= DateTime.Now;

            return insert.ExecuteAsync(task.Id,
                task.Type,
                task.WorkableId,
                task.WorkableRevision,
                task.WorkableName,
                task.Parameters.Serialize(),
                task.Started,
                task.Finished,
                task.Status,
                task.Result.Serialize(),
                task.Log.Serialize(),
                task.Performance.Serialize());
        }

        /// <inheritdoc />
        public async Task<WorkableTask> GetTask(Guid id) {
            if (runningtasks.TryGetValue(id, out WorkableTask task))
                return task;

            TaskDb dbtask = await loadtask.ExecuteEntityAsync(id);
            if (dbtask == null)
                throw new NotFoundException(typeof(WorkableTask), id.ToString());

            return new WorkableTask {
                Id = dbtask.Id,
                WorkableId = dbtask.WorkableId,
                WorkableRevision = dbtask.WorkableRevision,
                WorkableName = dbtask.WorkableName,
                Parameters = dbtask.Parameters.Deserialize<Dictionary<string, object>>(),
                Status = dbtask.Status,
                Started = dbtask.Started,
                Finished = dbtask.Finished,
                Runtime = dbtask.Finished - dbtask.Started,
                Result = dbtask.Result.Deserialize<object>(),
                Log = new List<string>(dbtask.Log.Deserialize<string[]>() ?? new string[0]),
                Performance = new List<ProfilingEntry>(dbtask.Performance.Deserialize<ProfilingEntry[]>() ?? new ProfilingEntry[0])
            };
        }

        WorkableTask GenerateListVersion(WorkableTask task) {
            return new WorkableTask {
                Id = task.Id,
                WorkableId = task.WorkableId,
                WorkableRevision = task.WorkableRevision,
                WorkableName = task.WorkableName,
                Started = task.Started,
                Finished = task.Finished,
                Runtime = (task.Finished ?? DateTime.Now) - task.Started,
                Status = task.Status,
                Result = task.Result
            };
        }
        
        /// <inheritdoc />
        public async Task<Page<WorkableTask>> ListTasks(TaskFilter filter=null) {
            filter ??= new TaskFilter();
            if (!filter.Count.HasValue || filter.Count > 500)
                filter.Count = 500;

            List<WorkableTask> match=new List<WorkableTask>();
            
            if (filter.Status == null || filter.Status.Contains(TaskStatus.Running) || filter.Status.Contains(TaskStatus.Suspended))
                match.AddRange(runningtasks.Values.Take((int) filter.Count.Value).Select(GenerateListVersion));

            long left = filter.Count.Value - match.Count;
            if (left > 0 && ((filter.Status?.Length??0)==0 || filter.Status.Any(s => s != TaskStatus.Running && s != TaskStatus.Suspended))) {
                PredicateExpression<TaskDb> tasks = null;
                if (filter.Status?.Length > 0)
                    tasks &= t => t.Status.In(filter.Status);
                if (!string.IsNullOrEmpty(filter.WorkableName))
                    tasks &= t => t.WorkableName.Like(filter.WorkableName.TranslateWildcards());
                if (filter.WorkableId.HasValue)
                    tasks &= t => t.WorkableId == filter.WorkableId;
                if (filter.From.HasValue)
                    tasks &= t => t.Started >= filter.From;
                if (filter.To.HasValue)
                    tasks &= t => t.Finished <= filter.To;

                WorkableTask[] results = await database.Load<TaskDb>(t => t.Id, t => t.WorkableId, t => t.WorkableRevision, t => t.WorkableName, t => t.Started, t => t.Finished, t => t.Status, t => t.Result)
                    .ApplyFilter(filter)
                    .OrderBy(new OrderByCriteria(DB.Property<WorkableTask>(w => w.Started), false))
                    .Where(tasks?.Content)
                    .ExecuteTypesAsync(
                        t => new WorkableTask {
                            Id = t.GetValue<Guid>(0),
                            WorkableId = t.GetValue<long>(1),
                            WorkableRevision = t.GetValue<int>(2),
                            WorkableName = t.GetValue<string>(3),
                            Started = t.GetValue<DateTime>(4),
                            Finished = t.GetValue<DateTime>(5),
                            Runtime = t.GetValue<DateTime>(5) - t.GetValue<DateTime>(4),
                            Status = t.GetValue<TaskStatus>(6),
                            Result = t.GetValue<string>(7).Deserialize<object>()
                        }
                    );

                return Page<WorkableTask>.Create(
                    match.Concat(results).OrderByDescending(r=>r.Started).ToArray(),
                    runningtasks.Count + await database.Load<TaskDb>(t => DBFunction.Count()).Where(tasks?.Content).ExecuteScalarAsync<long>(),
                    filter.Continue
                );
            }

            return Page<WorkableTask>.Create(
                match.ToArray(),
                runningtasks.Count,
                filter.Continue
            );
        }

        /// <inheritdoc />
        public async Task FinishTask(Guid id) {
            if (!runningtasks.TryGetValue(id, out WorkableTask task))
                throw new NotFoundException(typeof(WorkableTask), id.ToString());

            await StoreTask(task);
            runningtasks.Remove(id, out WorkableTask _);
        }

        /// <inheritdoc />
        public async Task<WorkableTask> CancelTask(Guid id) {
            if(!runningtasks.TryGetValue(id, out WorkableTask task))
                throw new NotFoundException(typeof(WorkableTask), id.ToString());

            if (task.Status == TaskStatus.Suspended) {
                task.Status = TaskStatus.Canceled;
                await FinishTask(id);
            }
            else {
                task.Token.Cancel();
                await Task.WhenAny(task.Task, Task.Delay(1000));
            }
            return task;
        }
    }
}