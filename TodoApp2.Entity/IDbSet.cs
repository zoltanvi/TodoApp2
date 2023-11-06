using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TodoApp2.Entity.Model;
using TodoApp2.Entity.Query;

namespace TodoApp2.Entity
{
    public interface IDbSet<TModel>
        where TModel : EntityModel
    {
        bool IsEmpty { get; }

        List<TModel> Where(Expression<Func<TModel, object>> whereExpression = null, int limit = QueryBuilder.NoLimit);
        List<TModel> GetAll(Expression<Func<TModel, object>> whereExpression = null, int limit = QueryBuilder.NoLimit);
        TModel First(Expression<Func<TModel, object>> whereExpression = null);
        
        bool Add(TModel model, bool writeAllProperties = false);
        void AddRange(IEnumerable<TModel> models, bool writeAllProperties = false);
        
        bool UpdateFirst(TModel model);
        int Update(TModel model, Expression<Func<TModel, object>> whereExpression = null, int limit = QueryBuilder.NoLimit);
        int UpdateRange(IEnumerable<TModel> models, Expression<Func<TModel, object>> sourceProperty);
        
        int Remove(Expression<Func<TModel, object>> whereExpression = null, int limit = QueryBuilder.NoLimit);
        bool RemoveFirst(Expression<Func<TModel, object>> whereExpression = null);
        
        int Count();
    }
}
