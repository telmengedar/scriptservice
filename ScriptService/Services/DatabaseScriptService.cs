using System.Threading.Tasks;
using NightlyCode.AspNetCore.Services.Data;
using NightlyCode.AspNetCore.Services.Errors.Exceptions;
using NightlyCode.Database.Clients;
using NightlyCode.Database.Entities;
using NightlyCode.Database.Entities.Operations;
using NightlyCode.Database.Entities.Operations.Prepared;
using NightlyCode.Database.Fields;
using ScriptService.Dto;
using ScriptService.Dto.Patches;
using ScriptService.Extensions;

namespace ScriptService.Services {

    /// <summary>
    /// stores and retrieves scripts using a database
    /// </summary>
    public class DatabaseScriptService : IScriptService {
        readonly IEntityManager database;
        readonly IArchiveService archiveservice;

        readonly PreparedOperation insertscript;
        readonly PreparedLoadOperation<Script> getscript;
        readonly PreparedLoadOperation<Script> getscriptbyname;
        readonly PreparedOperation deletescript;

        /// <summary>
        /// creates a new <see cref="DatabaseScriptService"/>
        /// </summary>
        /// <param name="database">access to database</param>
        /// <param name="archiveservice">access to object archive</param>
        public DatabaseScriptService(IEntityManager database, IArchiveService archiveservice) {
            this.database = database;
            this.archiveservice = archiveservice;
            database.UpdateSchema<Script>();
            insertscript = database.Insert<Script>().Columns(s => s.Revision, s => s.Name, s => s.Code).ReturnID().Prepare();
            getscript = database.Load<Script>().Where(s => s.Id == DBParameter.Int64).Prepare();
            getscriptbyname = database.Load<Script>().Where(s => s.Name == DBParameter.String).Prepare();
            deletescript = database.Delete<Script>().Where(s => s.Id == DBParameter.Int64).Prepare();
        }

        /// <inheritdoc />
        public Task<long> CreateScript(ScriptData script) {
            return insertscript.ExecuteAsync(1, script.Name, script.Code);
        }

        /// <inheritdoc />
        public async Task<Script> GetScript(long scriptid) {
            Script script = await getscript.ExecuteEntityAsync(scriptid);
            if (script == null)
                throw new NotFoundException(typeof(Script), scriptid);
            return script;
        }

        /// <inheritdoc />
        public async Task<Script> GetScript(string scriptname) {
            Script script = await getscriptbyname.ExecuteEntityAsync(scriptname);
            if(script == null)
                throw new NotFoundException(typeof(Script), scriptname);
            return script;
        }

        /// <inheritdoc />
        public async Task<Page<Script>> ListScripts(ListFilter filter = null) {
            filter ??= new ListFilter();
            LoadOperation<Script> operation = database.Load<Script>();
            operation.ApplyFilter(filter);
            return Page<Script>.Create(
                await operation.ExecuteEntitiesAsync(),
                await database.Load<Script>(s => DBFunction.Count()).ExecuteScalarAsync<long>(),
                filter.Continue
            );
        }

        /// <inheritdoc />
        public async Task PatchScript(long scriptid, PatchOperation[] patches) {
            Script script = await GetScript(scriptid);

            using Transaction transaction = database.Transaction();
            await archiveservice.ArchiveObject(transaction, script.Id, script.Revision, script);
            await database.Update<Script>().Set(s => s.Revision == s.Revision + 1).Where(s => s.Id == scriptid).Patch(patches).ExecuteAsync(transaction);
            transaction.Commit();
        }

        /// <inheritdoc />
        public async Task DeleteScript(long scriptid) {
            Script script = await GetScript(scriptid);

            using Transaction transaction = database.Transaction();
            await archiveservice.ArchiveObject(transaction, script.Id, script.Revision, script);
            await deletescript.ExecuteAsync(transaction, scriptid);
            transaction.Commit();
        }
    }
}