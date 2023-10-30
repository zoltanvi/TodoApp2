using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TodoApp2.Common;
using TodoApp2.Entity.Info;
using TodoApp2.Entity.Query;

namespace TodoApp2.Entity
{
    public class DbSet<TModel> 
        where TModel : class, new()
    {
        private DbConnection _connection;

        internal string TableName;
        internal DbSetMapping<TModel> DbSetMapping;

        /// <summary>
        /// Creates an abstract representation of an actual database table.
        /// </summary>
        /// <param name="connection">The db connection.</param>
        /// <param name="dbSetMapping">The db set mapping.</param>
        public DbSet(DbConnection connection, DbSetMapping<TModel> dbSetMapping)
        {
            ThrowHelper.ThrowIfNull(connection);
            ThrowHelper.ThrowIfNull(dbSetMapping);

            _connection = connection;
            DbSetMapping = dbSetMapping;
            TableName = dbSetMapping.TableName;
            DbSetMapping.CheckEmptyMapping();
            DbSetMapping.CreateIfNotExists(connection);
        }

        public List<TModel> GetAll(Expression<Func<TModel, object>> sourceProperty = null, int limit = QueryBuilder.NoLimit)
        {
            return QueryExecutor.GetItemsWithQuery<TModel>(
                _connection,
                WhereBuilder.SelectAll(TableName, sourceProperty, limit));
        }

        public TModel GetFirst(Expression<Func<TModel, object>> sourceProperty = null)
        {
            return QueryExecutor.GetItemWithQuery<TModel>(
                _connection,
                WhereBuilder.SelectAll(TableName, sourceProperty, QueryBuilder.Single));
        }

        public int Count()
        {
            var valueModel = QueryExecutor.GetItemWithQuery<SingleIntModel>(
                _connection, 
                QueryBuilder.SelectCount(TableName));

            return valueModel.Value;
        }

        // Create
        public bool Add(TModel model)
        {
            var primaryKeyName = DbSetMapping.ModelBuilder.GetPrimaryKeyName();

            bool success = QueryExecutor.InsertItem<TModel>(
                _connection, 
                model, 
                TableName, 
                primaryKeyName);

            return success;
        }

        // Update
        


        // Delete


    }
}
