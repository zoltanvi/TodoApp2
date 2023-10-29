using System;

namespace TodoApp2.Entity.Migration
{
    public abstract class DbMigration
    {
        /// <summary>
        /// The version of the migration. 
        /// After the migration is executed, the database is set to this version.
        /// </summary>
        public int Version { get; }

        protected internal MigrationBuilder MigrationBuilder { get; } = new MigrationBuilder();

        public DbMigration(int version)
        {
            if (version <= 0)
            {
                throw new ArgumentException("Version must be greater than 0!");
            }

            Version = version;
        }

        /// <summary>
        /// Builds the migration that contains the steps 
        /// to migrate the database to the <see cref="Version"/> 
        /// from the previous <see cref="Version"/>.
        /// </summary>
        public abstract void Up();
    }
}
