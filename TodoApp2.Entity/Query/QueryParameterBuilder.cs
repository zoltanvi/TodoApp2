using System;
using System.Data.SQLite;
using System.Linq;
using System.Linq.Expressions;
using TodoApp2.Entity.Extensions;
using TodoApp2.Entity.Model;

namespace TodoApp2.Entity.Query
{
    internal static class QueryParameterBuilder
    {
        public static SQLiteParameter[] ModelParameters<TModel>(TModel model, string primaryKeyName, bool writeAllProperties = false)
            where TModel : EntityModel
        {
            var modelType = typeof(TModel);

            if (writeAllProperties) primaryKeyName = null;
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
