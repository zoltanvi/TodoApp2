using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp2.Core
{
    public class CompatibilityDataAccess : BaseDataAccess
    {
        private const string GetVersionCommand = "PRAGMA user_version; ";

        public CompatibilityDataAccess(SQLiteConnection connection) : base(connection)
        {
        }

        public int ReadDbVersion()
        {
            int version = 0;

            // Get user version
            using (SQLiteCommand dbCommand = new SQLiteCommand(GetVersionCommand, m_Connection))
            using (SQLiteDataReader reader = dbCommand.ExecuteReader())
            {
                if (reader.Read())
                {
                    version = reader.SafeGetInt(Column.DatabaseVersion);
                }
            }

            return version;
        }

        public void UpdateDbVersion(int version)
        {
            ExecuteCommand($"PRAGMA user_version = {version}; ");
        }

        public void AddBorderColorToTaskTable()
        {
            ExecuteCommand($"ALTER TABLE {Table.Task} ADD COLUMN {Column.BorderColor} TEXT DEFAULT \"Transparent\" ");
        }

        internal void AddBackgroundColorToTaskTable()
        {
            ExecuteCommand($"ALTER TABLE {Table.Task} ADD COLUMN {Column.BackgroundColor} TEXT DEFAULT \"Transparent\" ");
        }
    }
}
