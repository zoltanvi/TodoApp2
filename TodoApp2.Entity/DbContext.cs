using System;
using System.Data.SQLite;
using TodoApp2.Common;

namespace TodoApp2.Entity
{
    /// <summary>
    /// Provides an abstraction over the actual database.
    /// The defined <see cref="DbSet{TModel}"/> properties represents actual tables in the database.
    /// </summary>
    public class DbContext : IDisposable
    {
        private string _connectionString;

        protected DbConnection Connection { get; }

        public  DbContext(string connectionString)
        {
            connectionString.ThrowIfEmpty();

            _connectionString = connectionString;
            Connection = new DbConnection(connectionString);
            Connection.Open();
        }

        public void Dispose()
        {
            Connection.Dispose();
        }
    }
}
