using System;
using System.Data.SQLite;
using System.Linq;
using System.Linq.Expressions;
using TodoApp2.Entity.Extensions;

namespace TodoApp2.Entity.Query
{
    internal static class QueryParameterBuilder
    {
        public static SQLiteParameter[] ModelParameters<TModel>(TModel model, string primaryKeyName)
            where TModel : class, new()
        {
            var modelType = typeof(TModel);
            var properties = modelType.GetPublicPropertiesWithExclusion(primaryKeyName);

            var parameters = new SQLiteParameter[properties.Count];

            for (int i = 0; i < properties.Count; i++)
            {
                var property = properties[i];
                parameters[i] = new SQLiteParameter($"@{property.Name}", property.GetValue(model));
            }

            return parameters;
        }
    }
}
