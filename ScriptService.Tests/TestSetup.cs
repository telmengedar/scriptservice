using Microsoft.Data.Sqlite;
using NightlyCode.Database.Clients;
using NightlyCode.Database.Entities;
using NightlyCode.Database.Info;

namespace ScriptService.Tests {

    /// <summary>
    /// setup tasks for tests
    /// </summary>
    public static class TestSetup {

        /// <summary>
        /// creates a new in memory database
        /// </summary>
        /// <returns>entity manager for database access</returns>
        public static IEntityManager CreateMemoryDatabase() {
            return new EntityManager(ClientFactory.Create(new SqliteConnection("Data Source=:memory:"), new SQLiteInfo()));
        }
    }
}