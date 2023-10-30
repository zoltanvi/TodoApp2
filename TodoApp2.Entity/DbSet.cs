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
        private string _primaryKey;
        private string PrimaryKey => _primaryKey ?? (_primaryKey = DbSetMapping.ModelBuilder.GetPrimaryKeyName());

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

        public List<TModel> GetAll(Expression<Func<TModel, object>> whereExpression = null, int limit = QueryBuilder.NoLimit)
        {
            return QueryExecutor.GetItemsWithQuery<TModel>(
                _connection,
                QueryBuilder.SelectAll(TableName, whereExpression, limit));
        }

        public TModel GetFirst(Expression<Func<TModel, object>> whereExpression = null)
        {
            return QueryExecutor.GetItemWithQuery<TModel>(
                _connection,
                QueryBuilder.SelectAll(TableName, whereExpression, QueryBuilder.Single));
        }

        public int Count()
        {
            var valueModel = QueryExecutor.GetItemWithQuery<SingleIntModel>(
                _connection, 
                QueryBuilder.SelectCount(TableName));

            return valueModel.Value;
        }

        public bool Add(TModel model)
        {
            int successful = QueryExecutor.ExecuteQuery(
                _connection,
                QueryBuilder.InsertInto<TModel>(TableName, PrimaryKey),
                QueryParameterBuilder.ModelParameters<TModel>(model, PrimaryKey));

            return successful == 1;
        }

        // Update
        
        public bool Update(TModel model, Expression<Func<TModel, object>> whereExpression = null, int limit = QueryBuilder.NoLimit)
        {
            int successful = QueryExecutor.ExecuteQuery(
                _connection,
                QueryBuilder.UpdateItem<TModel>(TableName, PrimaryKey, whereExpression, limit),
                QueryParameterBuilder.ModelParameters<TModel>(model, PrimaryKey));

            return successful == 1;
        }


        // Delete


    }
}
