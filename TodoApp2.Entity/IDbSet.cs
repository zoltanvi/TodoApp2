using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TodoApp2.Entity.Query;

namespace TodoApp2.Entity
{
    public interface IDbSet<TModel>
        where TModel : class, new()
    {
        bool IsEmpty { get; }

        List<TModel> Where(Expression<Func<TModel, object>> whereExpression = null, int limit = QueryBuilder.NoLimit);
        //List<TModel> Where(
        //    Expression<Func<TModel, object>> whereExpression = null,
        //    Expression<Func<TModel, object>> whereExpression2 = null,
        //    Expression<Func<TModel, object>> whereExpression3 = null,
        //    Expression<Func<TModel, object>> whereExpression4 = null,
        //    int limit = QueryBuilder.NoLimit);

        List<TModel> GetAll(Expression<Func<TModel, object>> whereExpression = null, int limit = QueryBuilder.NoLimit);
        TModel First(Expression<Func<TModel, object>> whereExpression = null);
        
        bool Add(TModel model);
        void AddRange(IEnumerable<TModel> models);
        
        bool UpdateFirst(TModel model, Expression<Func<TModel, object>> whereExpression = null);
        int Update(TModel model, Expression<Func<TModel, object>> whereExpression = null, int limit = QueryBuilder.NoLimit);
        int UpdateRange(IEnumerable<TModel> models, Expression<Func<TModel, object>> sourceProperty);
        
        int Remove(Expression<Func<TModel, object>> whereExpression = null, int limit = QueryBuilder.NoLimit);
        bool RemoveFirst(Expression<Func<TModel, object>> whereExpression = null);
        
        int Count();
    }
}
