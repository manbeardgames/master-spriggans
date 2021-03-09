using Microsoft.EntityFrameworkCore;

namespace MasterSpriggans.Data
{
    public class DBFactory : IDBFactory
    {
        //  The connection string used to connect to the SQLite Database file
        protected readonly string _connectionString;

        /// <summary>
        ///     Creates a new <see cref="DBFactory"/> instance.
        /// </summary>
        /// <param name="connectionString">
        ///     The connection string used to open the connection to the SQLite database file.
        /// </param>
        public DBFactory(string connectionString) => _connectionString = connectionString;

        /// <summary>
        ///     Creates and returns a new conneciton to the database context.
        /// </summary>
        /// <returns></returns>
        public MasterSpriggansDatabaseContext GetConnection()
        {
            var optionsBuilder = new DbContextOptionsBuilder<MasterSpriggansDatabaseContext>();
            optionsBuilder.UseSqlite(_connectionString);
            return new MasterSpriggansDatabaseContext(optionsBuilder.Options);
        }
    }
}