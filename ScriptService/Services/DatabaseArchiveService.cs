using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using NightlyCode.AspNetCore.Services.Data;
using NightlyCode.AspNetCore.Services.Errors.Exceptions;
using NightlyCode.Database.Entities;
using NightlyCode.Database.Entities.Operations.Prepared;
using NightlyCode.Database.Fields;
using ScriptService.Dto;
using ScriptService.Extensions;
using Utf8Json;

namespace ScriptService.Services {
    /// <inheritdoc />
    public class DatabaseArchiveService : IArchiveService {
        readonly IEntityManager database;

        readonly PreparedOperation insert;
        readonly PreparedLoadEntitiesOperation<ArchivedObject> load;

        /// <summary>
        /// creates a new <see cref="DatabaseArchiveService"/>
        /// </summary>
        /// <param name="database">access to database</param>
        public DatabaseArchiveService(IEntityManager database) {
            this.database = database;
            database.UpdateSchema<ArchivedObject>();
            insert = database.Insert<ArchivedObject>().Columns(o => o.Type, o => o.ObjectId, o => o.Revision, o => o.Data).Prepare();
            load = database.LoadEntities<ArchivedObject>().Where(o => o.Type == DBParameter.String && o.ObjectId == DBParameter.Int64 && o.Revision == DBParameter.Int32).Prepare();
        }

        /// <inheritdoc />
        public async Task ArchiveObject<T>(string type, long id, int revision, T objectdata) {
            byte[] serialized = JsonSerializer.Serialize(objectdata);
            await using MemoryStream output = new MemoryStream();
            await using (GZipStream gzip = new GZipStream(output, CompressionLevel.Optimal)) {
                gzip.Write(serialized);
            }

            await insert.ExecuteAsync(type, id, revision, output.ToArray());
        }

        /// <inheritdoc />
        public async Task<T> GetArchivedObject<T>(string type, long id, int revision) {
            ArchivedObject data = (await load.ExecuteAsync(type, id, revision)).FirstOrDefault();

            if (data == null)
                throw new NotFoundException(typeof(ArchivedObject), $"{type}/{id}.{revision}");

            await using MemoryStream source = new MemoryStream(data.Data);
            await using GZipStream gzip = new GZipStream(source, CompressionMode.Decompress);

            return JsonSerializer.Deserialize<T>(gzip);
        }

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