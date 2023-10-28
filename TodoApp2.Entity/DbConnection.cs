using System;
using System.Data.SQLite;

namespace TodoApp2.Entity
{
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
    }
}
