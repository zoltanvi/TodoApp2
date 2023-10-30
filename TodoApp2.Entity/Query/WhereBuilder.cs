using System;
using System.Linq.Expressions;
using TodoApp2.Entity.Extensions;

namespace TodoApp2.Entity.Query
{
    public static class WhereBuilder
    {
        internal static string SelectAll<TModel>(string tableName, Expression<Func<TModel, object>> sourceProperty, int limit)
            where TModel : class, new()
        {
            if (sourceProperty == null)
            {
                return QueryBuilder.SelectAll(tableName, whereSqlCondition: null, limit);
            }

            if (sourceProperty.Body is UnaryExpression unaryExpression &&
                unaryExpression.Operand is BinaryExpression binaryExpression &&
                binaryExpression.Left is MemberExpression memberExpression &&
                binaryExpression.Right is ConstantExpression constantExpression &&
                binaryExpression.NodeType == ExpressionType.Equal)
            {
                string propertyName = memberExpression.Member.Name;
                object propertyValue = constantExpression.Value;

                var modelType = typeof(TModel);
                var propType = modelType.GetPublicProperty(propertyName).PropertyType;

                string whereCondition = $"{propertyName} = ";

                if (propType == typeof(bool)) whereCondition += $"{propertyValue}";
                else if (propType == typeof(int)) whereCondition += $"{propertyValue}";
                else if (propType == typeof(long)) whereCondition += $"{propertyValue}";
                else if (propType == typeof(string)) whereCondition += $"'{propertyValue}'";

                return QueryBuilder.SelectAll(tableName, whereCondition, limit);
            }
            else
            {
                // TODO: implement greater than and less than if needed
                throw new ArgumentException("Expression must be in the format x => x.Property == objectValue");
            }
        }
    }
}
