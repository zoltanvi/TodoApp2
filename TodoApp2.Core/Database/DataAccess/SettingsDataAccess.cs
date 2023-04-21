using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace TodoApp2.Core
{
    // CRUD = Create, Read, Update, Delete
    public class SettingsDataAccess : BaseDataAccess
    {
        public SettingsDataAccess(SQLiteConnection connection) : base(connection)
        {
        }

        // Create

        public void CreateSettingsTable()
        {
            string createSettingsTable =
               $"CREATE TABLE IF NOT EXISTS {Table.Settings} ( " +
               $" {Column.Id} INTEGER PRIMARY KEY, " +
               $" {Column.Key} TEXT, " +
               $" {Column.Value} TEXT " +
               $"); ";

            ExecuteCommand(createSettingsTable);
        }

        public void AddDefaultSettingsIfNotExists()
        {
            const string windowLeftPos = "WindowLeftPos";
            const string windowTopPos = "WindowTopPos";
            const string windowWidth = "WindowWidth";
            const string windowHeight = "WindowHeight";
            const string activeCategoryId = "ActiveCategoryId";


            List<Setting> defaultSettings = new List<Setting>
            {
                new Setting {Id = 0, Key = windowLeftPos, Value = "100"},
                new Setting {Id = 1, Key = windowTopPos, Value = "100"},
                new Setting {Id = 2, Key = windowWidth, Value = "380"},
                new Setting {Id = 3, Key = windowHeight, Value = "500"},
                new Setting {Id = 4, Key = activeCategoryId, Value = "0"}
            };

            List<Setting> settings = GetSettings();

            // If none of the default settings are in the database, insert them
            if (!ContainsDefaultSettings(settings, defaultSettings))
            {
                AddSettings(defaultSettings);
            }
        }

        public void AddSetting(Setting settings)
        {
            using (SQLiteCommand command = new SQLiteCommand(m_Connection))
            {
                command.CommandText = $"INSERT INTO {Table.Settings} ({Column.Id}, {Column.Key}, {Column.Value}) " +
                                      $" VALUES ({Parameter.Id}, {Parameter.Key}, {Parameter.Value});";

                command.Parameters.AddRange(new[]
                {
                    new SQLiteParameter(Parameter.Id, settings.Id),
                    new SQLiteParameter(Parameter.Key, settings.Key),
                    new SQLiteParameter(Parameter.Value, settings.Value)
                });

                command.ExecuteNonQuery();
            }
        }

        public void AddSettings(IEnumerable<Setting> settingsList)
        {
            // Using transaction to write the database only once
            using (SQLiteTransaction transaction = m_Connection.BeginTransaction())
            {
                foreach (Setting settings in settingsList)
                {
                    AddSetting(settings);
                }

                transaction.Commit();
            }
        }

        // Read

        public List<Setting> GetSettings()
        {
            List<Setting> items = new List<Setting>();

            using (SQLiteCommand command = new SQLiteCommand(m_Connection))
            {
                command.CommandText = $"SELECT * FROM {Table.Settings}";

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new Setting
                        {
                            Id = reader.SafeGetInt(Column.Id),
                            Key = reader.SafeGetString(Column.Key),
                            Value = reader.SafeGetString(Column.Value)
                        });
                    }
                }
            }

            return items;
        }

        public int GetNextId()
        {
            int nextId = int.MinValue;

            using (SQLiteCommand command = new SQLiteCommand(m_Connection))
            {
                command.CommandText = 
                    $"SELECT * FROM {Table.Settings} " +
                    $"ORDER BY {Column.Id} DESC LIMIT 1";

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        nextId = reader.SafeGetInt(Column.Id) + 1;
                        break;
                    }
                }
            }

            return nextId;
        }

        // Update

        public int UpdateSettings(IEnumerable<Setting> settingsList)
        {
            int modifiedItems = 0;

            // Using transaction to write the database only once
            using (SQLiteTransaction transaction = m_Connection.BeginTransaction())
            {
                foreach (var setting in settingsList)
                {
                    using (SQLiteCommand command = new SQLiteCommand(m_Connection))
                    {
                        command.CommandText = $"UPDATE {Table.Settings} SET " +
                                              $"  {Column.Value} = {Parameter.Value} " +
                                              $" WHERE {Column.Key} = {Parameter.Key};";

                        command.Parameters.AddRange(new[]
                        {
                            new SQLiteParameter(Parameter.Key, setting.Key),
                            new SQLiteParameter(Parameter.Value, setting.Value)
                        });

                        modifiedItems += command.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }

            return modifiedItems;
        }

        private bool ContainsDefaultSettings(IEnumerable<Setting> settingsList, List<Setting> defaultSettings)
        {
            foreach (Setting setting in settingsList)
            {
                if (defaultSettings.Any(defaultSetting => defaultSetting.Key == setting.Key))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
