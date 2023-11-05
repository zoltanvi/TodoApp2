using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TodoApp2.Common;
using TodoApp2.Entity.Info;
using TodoApp2.Entity.Query;

namespace TodoApp2.Entity
{
    public class DbSet<TModel> : IDbSet<TModel>
        where TModel : class, new()
    {
        private DbConnection _connection;
        private string _primaryKey;
        private string PrimaryKey => _primaryKey ?? (_primaryKey = DbSetMapping.ModelBuilder.GetPrimaryKeyName());

        public bool IsEmpty => Count() == 0;

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

        public List<TModel> Where(Expression<Func<TModel, object>> whereExpression = null, int limit = QueryBuilder.NoLimit) =>
            GetAll(whereExpression, limit);

        public List<TModel> GetAll(Expression<Func<TModel, object>> whereExpression = null, int limit = QueryBuilder.NoLimit) =>
            QueryExecutor.GetItemsWithQuery<TModel>(_connection, QueryBuilder.SelectAll(TableName, whereExpression, limit));

        public TModel First(Expression<Func<TModel, object>> whereExpression = null) =>
            QueryExecutor.GetItemWithQuery<TModel>(_connection, QueryBuilder.SelectAll(TableName, whereExpression, QueryBuilder.Single));

        public bool Add(TModel model) =>
            QueryExecutor.ExecuteQuery(
                _connection,
                QueryBuilder.InsertInto<TModel>(TableName, PrimaryKey),
                QueryParameterBuilder.ModelParameters<TModel>(model, PrimaryKey)) == QueryBuilder.Single;

        public void AddRange(IEnumerable<TModel> models)
        {
            using (var transaction = _connection.BeginTransaction())
            {
                foreach (TModel model in models)
                {
                    Add(model);
                }

                transaction.Commit();
            }
        }

        public bool UpdateFirst(TModel model, Expression<Func<TModel, object>> whereExpression = null) =>
            Update(model, whereExpression, QueryBuilder.Single) == QueryBuilder.Single;

        public int Update(TModel model, Expression<Func<TModel, object>> whereExpression = null, int limit = QueryBuilder.NoLimit) =>
            QueryExecutor.ExecuteQuery(
                _connection,
                QueryBuilder.UpdateItem<TModel>(TableName, PrimaryKey, whereExpression, limit),
                QueryParameterBuilder.ModelParameters<TModel>(model, PrimaryKey));

        public int UpdateRange(IEnumerable<TModel> models, Expression<Func<TModel, object>> sourceProperty)
        {
            int successful = 0;
            using (var transaction = _connection.BeginTransaction())
            {
                foreach(TModel model in models)
                {
                    successful += QueryExecutor.ExecuteQuery(
                        _connection,
                        QueryBuilder.UpdateItemFromModel<TModel>(TableName, PrimaryKey, model, sourceProperty),
                        QueryParameterBuilder.ModelParameters<TModel>(model, PrimaryKey));
                }

                transaction.Commit();
            }

            return successful;
        }

        public int Remove(Expression<Func<TModel, object>> whereExpression = null, int limit = QueryBuilder.NoLimit) =>
            QueryExecutor.ExecuteQuery(_connection, QueryBuilder.Delete<TModel>(TableName, whereExpression, limit));

        public bool RemoveFirst(Expression<Func<TModel, object>> whereExpression = null) =>
            Remove(whereExpression, QueryBuilder.Single) == QueryBuilder.Single;

        public int Count() => QueryExecutor.GetItemWithQuery<SingleIntModel>(_connection, QueryBuilder.SelectCount(TableName)).Value;
    }
}
