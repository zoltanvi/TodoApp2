using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TodoApp2.Common;
using TodoApp2.Entity.Info;
using TodoApp2.Entity.Model;
using TodoApp2.Entity.Query;

namespace TodoApp2.Entity
{
    public class DbSet<TModel> : IDbSet<TModel>
        where TModel : EntityModel, new()
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

        public List<TModel> Where(
            Expression<Func<TModel, object>> whereExpression = null,
            int limit = QueryBuilder.NoLimit)
        {
            return GetAll(whereExpression, limit);
        }

        public List<TModel> GetAll(
            Expression<Func<TModel, object>> whereExpression = null,
            int limit = QueryBuilder.NoLimit)
        {
            var query = QueryBuilder.SelectAll(TableName, whereExpression, limit);
            var items = QueryExecutor.GetItemsWithQuery<TModel>(_connection, query);
            return items;
        }

        public TModel First(
            Expression<Func<TModel, object>> whereExpression = null)
        {
            var query = QueryBuilder.SelectAll(TableName, whereExpression, QueryBuilder.Single);
            var item = QueryExecutor.GetItemWithQuery<TModel>(_connection, query);
            return item;
        }

        public bool Add(
            TModel model,
            bool writeAllProperties = false)
        {
            string query = QueryBuilder.InsertInto<TModel>(TableName, PrimaryKey, writeAllProperties);
            var parameters = QueryParameterBuilder.ModelParameters<TModel>(model, PrimaryKey, writeAllProperties);
            var result = QueryExecutor.ExecuteQuery(_connection, query, parameters);
            return result == QueryBuilder.Single;
        }

        public void AddRange(
            IEnumerable<TModel> models,
            bool writeAllProperties = false)
        {
            using (var transaction = _connection.BeginTransaction())
            {
                foreach (TModel model in models)
                {
                    Add(model, writeAllProperties);
                }

                transaction.Commit();
            }
        }

        public bool UpdateFirst(TModel model)
        {
            string query = QueryBuilder.UpdateItemWherePrimaryKeyMatches<TModel>(TableName, PrimaryKey, model);
            var parameters = QueryParameterBuilder.ModelParameters<TModel>(model, PrimaryKey);
            var result = QueryExecutor.ExecuteQuery(_connection, query, parameters);
            return result == QueryBuilder.Single;
        }

        public int Update(
            TModel model,
            Expression<Func<TModel, object>> whereExpression = null,
            int limit = QueryBuilder.NoLimit)
        {
            string query = QueryBuilder.UpdateItem<TModel>(TableName, PrimaryKey, whereExpression, limit);
            var parameters = QueryParameterBuilder.ModelParameters<TModel>(model, PrimaryKey);
            var result = QueryExecutor.ExecuteQuery(_connection, query, parameters);
            return result;
        }

        public int UpdateRange(
            IEnumerable<TModel> models,
            Expression<Func<TModel, object>> sourceProperty)
        {
            int successful = 0;
            using (var transaction = _connection.BeginTransaction())
            {
                foreach (TModel model in models)
                {
                    string query = QueryBuilder.UpdateItemFromModel<TModel>(TableName, PrimaryKey, model, sourceProperty);
                    var parameters = QueryParameterBuilder.ModelParameters<TModel>(model, PrimaryKey);

                    successful += QueryExecutor.ExecuteQuery(_connection, query, parameters);
                }

                transaction.Commit();
            }

            return successful;
        }

        public bool RemoveFirst(
            Expression<Func<TModel, object>> whereExpression = null)
        {
            return Remove(whereExpression, QueryBuilder.Single) == QueryBuilder.Single;
        }

        public int Remove(
            Expression<Func<TModel, object>> whereExpression = null,
            int limit = QueryBuilder.NoLimit)
        {
            string query = QueryBuilder.Delete<TModel>(TableName, whereExpression, limit);
            var result = QueryExecutor.ExecuteQuery(_connection, query);
            return result;
        }

        public int Count()
        {
            string query = QueryBuilder.SelectCount(TableName);

            SingleIntModel result = QueryExecutor.GetItemWithQuery<SingleIntModel>(_connection, query);

            return result.Value;
        }

    }
}
