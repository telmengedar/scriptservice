using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using NightlyCode.AspNetCore.Services.Data;
using NightlyCode.AspNetCore.Services.Errors.Exceptions;
using NightlyCode.Database.Clients;
using NightlyCode.Database.Entities;
using NightlyCode.Database.Entities.Operations.Prepared;
using NightlyCode.Database.Fields;
using NightlyCode.Json;
using ScriptService.Dto;
using ScriptService.Extensions;

namespace ScriptService.Services {
    /// <inheritdoc />
    public class DatabaseArchiveService : IArchiveService {
        readonly IEntityManager database;

        readonly PreparedOperation insert;
        readonly PreparedLoadOperation<ArchivedObject> load;

        /// <summary>
        /// creates a new <see cref="DatabaseArchiveService"/>
        /// </summary>
        /// <param name="database">access to database</param>
        public DatabaseArchiveService(IEntityManager database) {
            this.database = database;
            database.UpdateSchema<ArchivedObject>();
            insert = database.Insert<ArchivedObject>().Columns(o => o.Type, o => o.ObjectId, o => o.Revision, o => o.Data).Prepare();
            load = database.Load<ArchivedObject>().Where(o => o.Type == DBParameter.String && o.ObjectId == DBParameter.Int64 && o.Revision == DBParameter.Int32).Prepare();
        }

        /// <inheritdoc />
        public async Task ArchiveObject<T>(Transaction transaction, long id, int revision, T objectdata, string typename=null) {
            typename ??= typeof(T).Name;
            await using MemoryStream output = new MemoryStream();
            await using (GZipStream gzip = new GZipStream(output, CompressionLevel.Optimal)) {
                await Json.WriteAsync(objectdata, gzip);
            }

            await insert.ExecuteAsync(transaction, typename, id, revision, output.ToArray());
        }

        /// <inheritdoc />
        public async Task<T> GetArchivedObject<T>(long id, int revision, string typename=null) {
            typename ??= typeof(T).Name;
            
            ArchivedObject data = await load.ExecuteEntityAsync(typename, id, revision);

            if (data == null)
                throw new NotFoundException(typeof(ArchivedObject), $"{typeof(T).Name}/{id}.{revision}");

            await using MemoryStream source = new MemoryStream(data.Data);
            await using GZipStream gzip = new GZipStream(source, CompressionMode.Decompress);

            return Json.Read<T>(gzip);
        }

        /// <inheritdoc />
        public async Task<Page<int>> ListRevisions(string type, long id, ListFilter filter = null) {
            filter??=new ListFilter();
            return Page<int>.Create(
                await database.Load<ArchivedObject>(o => o.Revision).Where(o => o.Type == type && o.ObjectId == id).ApplyFilter(filter).ExecuteSetAsync<int>(),
                await database.Load<ArchivedObject>(o => DBFunction.Count()).Where(o => o.Type == type && o.ObjectId == id).ExecuteScalarAsync<long>(),
                filter.Continue
            );
        }
    }
}