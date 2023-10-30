using System;
using System.Data.SQLite;

namespace TodoApp2.Entity
{
    // Wrapper for SQLiteConnection to hide it from outside of the Entity project.
    // See: DbSet ctor
    public class DbConnection : IDisposable
    {
        internal SQLiteConnection InternalConnection { get; }

        public DbConnection(string connectionString)
        {
            InternalConnection = new SQLiteConnection(connectionString);
        }

        public void Dispose()
        {
            InternalConnection.Dispose();
        }

        internal void Open()
        {
            InternalConnection.Open();
        }

        internal SQLiteTransaction BeginTransaction() => InternalConnection.BeginTransaction();
    }
}
