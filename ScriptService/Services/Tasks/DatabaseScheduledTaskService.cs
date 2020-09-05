using System;
using System.Linq;
using System.Threading.Tasks;
using NightlyCode.AspNetCore.Services.Data;
using NightlyCode.AspNetCore.Services.Errors.Exceptions;
using NightlyCode.Database.Entities;
using NightlyCode.Database.Entities.Operations;
using NightlyCode.Database.Entities.Operations.Prepared;
using NightlyCode.Database.Expressions;
using NightlyCode.Database.Fields;
using ScriptService.Dto;
using ScriptService.Dto.Patches;
using ScriptService.Dto.Tasks;
using ScriptService.Extensions;

namespace ScriptService.Services.Tasks {

    /// <summary>
    /// uses database to handle <see cref="ScheduledTask"/>s
    /// </summary>
    public class DatabaseScheduledTaskService : IScheduledTaskService {
        readonly IEntityManager database;

        readonly PreparedOperation insert;
        readonly PreparedLoadEntitiesOperation<ScheduledTask> loadbyid;
        readonly PreparedOperation delete;
        readonly PreparedOperation updatescheduletime;
        readonly PreparedOperation updateexecution;

        /// <summary>
        /// creates a new <see cref="DatabaseScheduledTaskService"/>
        /// </summary>
        /// <param name="database"></param>
        public DatabaseScheduledTaskService(IEntityManager database) {
            this.database = database;
            database.UpdateSchema<ScheduledTask>();
            insert = database.Insert<ScheduledTask>().Columns(t => t.Name, t => t.WorkableType, t => t.WorkableName, t => t.WorkableRevision, t => t.Target, t => t.Days, t => t.Interval).Prepare();
            delete = database.Delete<ScheduledTask>().Where(t => t.Id == DBParameter.Int64).Prepare();
            loadbyid = database.LoadEntities<ScheduledTask>().Where(t => t.Id == DBParameter.Int64).Prepare();
            updatescheduletime = database.Update<ScheduledTask>().Set(t => t.Target == DBParameter<DateTime?>.Value).Where(t => t.Id == DBParameter.Int64).Prepare();
            updateexecution = database.Update<ScheduledTask>().Set(t => t.LastExecution == DBParameter<DateTime?>.Value, t => t.Target == DBParameter<DateTime?>.Value).Where(t => t.Id == DBParameter.Int64).Prepare();
        }


        /// <inheritdoc />
        public Task<long> Create(ScheduledTaskData data) {
            ScheduledDays days = data.Days;
            if (days == ScheduledDays.None)
                days = ScheduledDays.All;
            DateTime? nextexecution = data.FirstExecutionTime();
            return insert.ExecuteAsync(data.Name, data.WorkableType, data.WorkableName, data.WorkableRevision, nextexecution, days, data.Interval);
        }

        /// <inheritdoc />
        public async Task<ScheduledTask> GetById(long id) {
            ScheduledTask task = (await loadbyid.ExecuteAsync(id)).FirstOrDefault();
            if (task == null)
                throw new NotFoundException(typeof(ScheduledTask), id);
            return task;
        }

        /// <inheritdoc />
        public async Task<Page<ScheduledTask>> List(ScheduledTaskFilter filter = null) {
            filter ??= new ScheduledTaskFilter();
            LoadEntitiesOperation<ScheduledTask> operation = database.LoadEntities<ScheduledTask>();
            operation.ApplyFilter(filter);

            PredicateExpression<ScheduledTask> predicate = null;
            if (filter.DueTime.HasValue)
                predicate &= t => t.Target <= filter.DueTime;

            if (predicate != null)
                operation.Where(predicate.Content);

            return Page<ScheduledTask>.Create(
                await operation.ExecuteAsync<ScheduledTask>(),
                await database.Load<ScheduledTask>(t => DBFunction.Count()).Where(predicate?.Content).ExecuteScalarAsync<long>(),
                filter.Continue);
        }

        /// <inheritdoc />
        public async Task Update(long id, PatchOperation[] patches) {
            if(await database.Update<ScheduledTask>().Patch(patches).Where(t=>t.Id==id).ExecuteAsync()==0)
                throw new NotFoundException(typeof(ScheduledTask), id);

            if (patches.Any(p => p.Path == "/interval" || p.Path == "/days")) {
                ScheduledTask task = await GetById(id);
                await Schedule(id, task.NextExecutionTime(task.LastExecution));
            }
        }

        /// <inheritdoc />
        public async Task Delete(long id) {
            if(await delete.ExecuteAsync(id)==0)
                throw new NotFoundException(typeof(ScheduledTask), id);
        }

        /// <inheritdoc />
        public async Task Schedule(long id, DateTime? time) {
            if(await updatescheduletime.ExecuteAsync(time, id)==0)
                throw new NotFoundException(typeof(ScheduledTask), id);
        }

        /// <inheritdoc />
        public async Task UpdateExecution(long id, DateTime? nextexecution) {
            if (await updateexecution.ExecuteAsync(DateTime.Now, nextexecution, id) == 0)
                throw new NotFoundException(typeof(ScheduledTask), id);
        }
    }
}