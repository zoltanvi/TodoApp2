using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace TodoApp2.Core
{
    // CRUD = Create, Read, Update, Delete
    public class CategoryDataAccess : BaseDataAccess
    {
        public CategoryDataAccess(SQLiteConnection connection) : base(connection)
        {
        }

        // Create

        public void CreateCategoryTable()
        {
            string createCategoryTable =
                $"CREATE TABLE IF NOT EXISTS {Table.Category} ( " +
                $" {Column.Id} INTEGER PRIMARY KEY, " +
                $" {Column.Name} TEXT, " +
                $" {Column.ListOrder} TEXT DEFAULT ('{DefaultListOrder}'), " +
                $" {Column.Trashed} INTEGER " +
                $"); ";

            ExecuteCommand(createCategoryTable);
        }

        public bool AddDefaultCategoryIfNotExists()
        {
            bool exists = true;

            // This should only happen when the application database is just created
            if (GetCategoryCount() == 0)
            {
                exists = false;

                AddCategory(new CategoryViewModel
                {
                    Id = 0,
                    Name = "Today",
                    ListOrder = DefaultListOrder
                });

                AddCategory(new CategoryViewModel
                {
                    Id = 1,
                    Name = "Help",
                    ListOrder = DefaultListOrder + 1
                });
            }
            return exists;
        }

        public void AddCategory(CategoryViewModel category)
        {
            using (SQLiteCommand command = new SQLiteCommand(m_Connection))
            {
                command.CommandText =
                    $"INSERT INTO {Table.Category} " +
                    $"({Column.Id}, {Column.Name}, {Column.ListOrder}, {Column.Trashed}) " +
                    $" VALUES ({Parameter.Id}, {Parameter.Name}, {Parameter.ListOrder}, {Parameter.Trashed});";

                command.Parameters.AddRange(new[]
                {
                    new SQLiteParameter(Parameter.Id, category.Id),
                    new SQLiteParameter(Parameter.Name, category.Name),
                    new SQLiteParameter(Parameter.ListOrder, category.ListOrder.ToString("D19")),
                    new SQLiteParameter(Parameter.Trashed, category.Trashed)
                });

                command.ExecuteNonQuery();
            }
        }

        // Read

        public List<CategoryViewModel> GetCategories()
        {
            string query =
                $"SELECT * FROM {Table.Category} " +
                $" ORDER BY {Column.ListOrder}";

            return GetItemsWithQuery(query, ReadCategory);
        }

        public List<CategoryViewModel> GetActiveCategories()
        {
            string query =
                $"SELECT * FROM {Table.Category} " +
                $" WHERE {Column.Trashed} = 0 " +
                $" ORDER BY {Column.ListOrder}";

            return GetItemsWithQuery(query, ReadCategory);
        }

        public CategoryViewModel GetCategory(string name)
        {
            CategoryViewModel item = null;

            using (SQLiteCommand command = new SQLiteCommand(m_Connection))
            {
                command.CommandText =
                    $"SELECT * FROM {Table.Category} " +
                    $"WHERE {Column.Name} = {Parameter.Name}";

                command.Parameters.Add(new SQLiteParameter(Parameter.Name, name));

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        item = ReadCategory(reader);
                    }
                }
            }

            return item;
        }

        public CategoryViewModel GetCategory(int categoryId)
        {
            CategoryViewModel item = null;

            using (SQLiteCommand command = new SQLiteCommand(m_Connection))
            {
                command.CommandText =
                    $"SELECT * FROM {Table.Category} " +
                    $"WHERE {Column.Id} = {Parameter.Id}";

                command.Parameters.Add(new SQLiteParameter(Parameter.Id, categoryId));

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        item = ReadCategory(reader);
                    }
                }
            }

            return item;
        }

        public int GetNextId()
        {
            int nextId = int.MinValue;

            using (SQLiteCommand command = new SQLiteCommand(m_Connection))
            {
                command.CommandText =
                    $"SELECT * FROM {Table.Category} " +
                    $"ORDER BY {Column.Id} DESC LIMIT 1";

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        nextId = reader.SafeGetInt(Column.Id) + 1;
                    }
                }
            }

            return nextId;
        }

        public long GetLastListOrder()
        {
            return GetListOrder(Column.Category, false);
        }

        // Update

        public bool UpdateCategory(CategoryViewModel category)
        {
            bool result = false;
            using (SQLiteCommand command = new SQLiteCommand(m_Connection))
            {
                command.CommandText = $"UPDATE {Table.Category} SET " +
                                      $" {Column.Name} = {Parameter.Name}, " +
                                      $" {Column.ListOrder} = {Parameter.ListOrder}, " +
                                      $" {Column.Trashed} = {Parameter.Trashed} " +
                                      $" WHERE {Column.Id} IS {Parameter.Id};";

                command.Parameters.AddRange(new[]
                {
                    new SQLiteParameter(Parameter.Name, category.Name),
                    new SQLiteParameter(Parameter.ListOrder, category.ListOrder.ToString("D19")),
                    new SQLiteParameter(Parameter.Trashed, category.Trashed),
                    new SQLiteParameter(Parameter.Id, category.Id)
                });

                result = command.ExecuteNonQuery() > 0;
            }

            return result;
        }

        public int UpdateCategoryListOrders(IEnumerable<CategoryViewModel> categoryList)
        {
            int modifiedItems = 0;

            // Using transaction to write the database only once
            using (SQLiteTransaction transaction = m_Connection.BeginTransaction())
            {
                foreach (var category in categoryList)
                {
                    using (SQLiteCommand command = new SQLiteCommand(m_Connection))
                    {
                        command.CommandText = $"UPDATE {Table.Category} SET " +
                                              $" {Column.ListOrder} = {Parameter.ListOrder} " +
                                              $" WHERE {Column.Id} = {Parameter.Id};";

                        command.Parameters.AddRange(new[]
                        {
                            new SQLiteParameter(Parameter.Id, category.Id),
                            new SQLiteParameter(Parameter.ListOrder, category.ListOrder.ToString("D19"))
                        });

                        modifiedItems += command.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }

            return modifiedItems;
        }

        private CategoryViewModel ReadCategory(SQLiteDataReader reader)
        {
            CategoryViewModel category = new CategoryViewModel
            {
                Id = reader.SafeGetInt(Column.Id),
                Name = reader.SafeGetString(Column.Name),
                ListOrder = reader.SafeGetLongFromString(Column.ListOrder),
                Trashed = reader.SafeGetBoolFromInt(Column.Trashed)
            };

            return category;
        }

        private int GetCategoryCount()
        {
            int count = 0;

            using (SQLiteCommand command = new SQLiteCommand(m_Connection))
            {
                command.CommandText = $"SELECT COUNT(*) FROM {Table.Category} ";

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        count = reader.GetInt32(0);
                    }
                }
            }

            return count;
        }
    }
}
