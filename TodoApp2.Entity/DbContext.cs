using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using TodoApp2.Common;
using TodoApp2.Entity.Info;
using TodoApp2.Entity.Migration;
using TodoApp2.Entity.Query;
using DbMigration = TodoApp2.Entity.Migration.DbMigration;

namespace TodoApp2.Entity
{
    /// <summary>
    /// Provides an abstraction over the actual database.
    /// The defined <see cref="DbSet{TModel}"/> properties represents actual tables in the database.
    /// </summary>
    public abstract class DbContext : IDisposable
    {
        private string _connectionString;
        private int _dbVersion;

        protected DbConnection Connection { get; private set; }
        protected List<DbMigration> Migrations { get; } = new List<DbMigration>();

        public DbContext(string connectionString)
        {
            connectionString.ThrowIfEmpty();
            _connectionString = connectionString;

            Initialize();

            CreateDbSets();
            AddMigrations();

            UpdateDbVersionIfNeeded();
            RunMigrations();
        }

        /// <summary>
        /// The DbSets needs to be added here
        /// to ensure that the database remains in a consistent state.
        /// </summary>
        public abstract void CreateDbSets();

        /// <summary>
        /// If the context has any migration, those needs to be added here
        /// to ensure that the database remains in a consistent state.
        /// </summary>
        public virtual void AddMigrations() { }

        public void AddMigration(DbMigration migration)
        {
            ThrowHelper.ThrowIfNull(migration);

            if (Migrations.Any(x => x.Version == migration.Version))
            {
                throw new InvalidOperationException("Can't add more than 1 migration for the same version!");
            }

            if (Migrations.Count != 0 && Migrations.Last().Version > migration.Version)
            {
                throw new InvalidOperationException("You must add the migrations in ascending version order!");
            }

            Migrations.Add(migration);
        }

        public void Dispose()
        {
            Connection.Dispose();
        }

        private void RunMigrations()
        {
            using (SQLiteTransaction transaction = Connection.InternalConnection.BeginTransaction())
            {
                try
                {
                    int updatedDbVersion = MigrationRunner.Run(Migrations, _dbVersion, Connection);
                    
                    transaction.Commit();

                    _dbVersion = updatedDbVersion;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }

                UpdateDbVersion();
            }
        }

        private void UpdateDbVersionIfNeeded()
        {
            // If the db version is 0 then it's a fresh db, already migrated to the latest version.
            if (_dbVersion == 0)
            {
                // The latest version is 1 if there is no migration for this context
                _dbVersion = Migrations.Count == 0 ? 1 : Migrations.Last().Version;
            }
        }

        private void Initialize()
        {
            Connection = new DbConnection(_connectionString);
            Connection.Open();

            // Turn foreign keys on
            QueryExecutor.ExecuteQuery(Connection, QueryBuilder.TurnForeignKeysOn);

            // Get database version
            var dbVersionModel = QueryExecutor.GetItemWithQuery<DbVersionModel>(Connection, QueryBuilder.GetDbVersion);
            _dbVersion = dbVersionModel?.user_version ?? 0;
        }

        private void UpdateDbVersion()
        {
            QueryExecutor.ExecuteQuery(Connection, QueryBuilder.UpdateDbVersion(_dbVersion));
        }

    }
}
