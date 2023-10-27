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
               $" {Column.Key} TEXT PRIMARY KEY, " +
               $" {Column.Value} TEXT " +
               $"); ";

            ExecuteCommand(createSettingsTable);
        }

        public void AddDefaultSettingsIfNotExists()
        {
            List<Setting> defaultSettings = new List<Setting>
            {
                new Setting($"{nameof(WindowSettings)}.{nameof(WindowSettings.Left)}", "100"),
                new Setting($"{nameof(WindowSettings)}.{nameof(WindowSettings.Top)}", "100"),
                new Setting($"{nameof(WindowSettings)}.{nameof(WindowSettings.Width)}", "400"),
                new Setting($"{nameof(WindowSettings)}.{nameof(WindowSettings.Height)}", "540"),
                new Setting($"{nameof(SessionSettings)}.{nameof(SessionSettings.ActiveCategoryId)}", "0"),
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
            using (SQLiteCommand command = new SQLiteCommand(_connection))
            {
                command.CommandText = $"INSERT INTO {Table.Settings} ({Column.Key}, {Column.Value}) " +
                                      $" VALUES ({Parameter.Key}, {Parameter.Value});";

                command.Parameters.AddRange(new[]
                {
                    new SQLiteParameter(Parameter.Key, settings.Key),
                    new SQLiteParameter(Parameter.Value, settings.Value)
                });

                command.ExecuteNonQuery();
            }
        }

        public void AddSettings(IEnumerable<Setting> settingsList)
        {
            // Using transaction to write the database only once
            using (SQLiteTransaction transaction = _connection.BeginTransaction())
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

            using (SQLiteCommand command = new SQLiteCommand(_connection))
            {
                command.CommandText = $"SELECT * FROM {Table.Settings}";

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new Setting(
                            reader.SafeGetString(Column.Key),
                            reader.SafeGetString(Column.Value)));
                    }
                }
            }

            return items;
        }

        public Setting GetSetting(string key)
        {
            Setting item = null;

            using (SQLiteCommand command = new SQLiteCommand(_connection))
            {
                command.CommandText = $"SELECT * FROM {Table.Settings}" +
                                      $" WHERE {Column.Key} = {Parameter.Key};";

                command.Parameters.AddRange(new[]
                {
                    new SQLiteParameter(Parameter.Key, key),
                });

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        item = new Setting(
                            reader.SafeGetString(Column.Key),
                            reader.SafeGetString(Column.Value));
                        break;
                    }
                }
            }

            return item;
        }

        // Update

        public int UpdateSettings(IEnumerable<Setting> settingsList)
        {
            int modifiedItems = 0;

            // Using transaction to write the database only once
            using (SQLiteTransaction transaction = _connection.BeginTransaction())
            {
                foreach (var setting in settingsList)
                {
                    using (SQLiteCommand command = new SQLiteCommand(_connection))
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

        public bool UpdateSetting(string settingKey, Setting newSetting)
        {
            bool result = false;

            using (SQLiteCommand command = new SQLiteCommand(_connection))
            {
                command.CommandText = $"UPDATE {Table.Settings} SET " +
                                      $"  {Column.Key} = {Parameter.NewKey}," +
                                      $"  {Column.Value} = {Parameter.Value} " +
                                      $" WHERE {Column.Key} = {Parameter.Key};";

                command.Parameters.AddRange(new[]
                {
                    new SQLiteParameter(Parameter.Key, settingKey),
                    new SQLiteParameter(Parameter.NewKey, newSetting.Key),
                    new SQLiteParameter(Parameter.Value, newSetting.Value)
                });

                result = command.ExecuteNonQuery() > 0;
            }

            return result;
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
