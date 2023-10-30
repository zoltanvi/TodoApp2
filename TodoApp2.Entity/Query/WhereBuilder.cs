using System;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq.Expressions;
using TodoApp2.Entity.Extensions;

namespace TodoApp2.Entity.Query
{
    internal static class WhereBuilder
    {
        public static string EqualsExpression<TModel>(Expression<Func<TModel, object>> whereExpression)
            where TModel : class, new()
        {
            if (whereExpression == null) return null;

            if (whereExpression.Body is UnaryExpression unaryExpression &&
                unaryExpression.Operand is BinaryExpression binaryExpression &&
                binaryExpression.Left is MemberExpression leftMemberExpression &&
                binaryExpression.NodeType == ExpressionType.Equal)
            {
                string propertyName = leftMemberExpression.Member.Name;

                object propertyValue = null;
                
                if (binaryExpression.Right is ConstantExpression constantExpression)
                {
                    propertyValue = constantExpression.Value;
                } 
                else if(binaryExpression.Right is MemberExpression rightMemberExpression)
                {
                    // Compile and execute the rightMemberExpression to get the value
                    var compiledExpression = Expression.Lambda(rightMemberExpression).Compile();
                    propertyValue = compiledExpression.DynamicInvoke();
                }
                else
                {
                    throw new ArgumentException("Expression must be in the format x => x.Property == value");
                }

                var modelType = typeof(TModel);
                var propType = modelType.GetPublicProperty(propertyName).PropertyType;

                string whereCondition = $"{propertyName} = ";

                if (propType == typeof(bool)) whereCondition += $"{propertyValue}";
                else if (propType == typeof(int)) whereCondition += $"{propertyValue}";
                else if (propType == typeof(long)) whereCondition += $"{propertyValue}";
                else if (propType == typeof(string)) whereCondition += $"'{propertyValue}'";

                return whereCondition;
            }
            else
            {
                // TODO: implement greater than and less than if needed
                throw new ArgumentException("Expression must be in the format x => x.Property == value");
            }
        }

        public static string EqualsWithModelExpression<TModel>(Expression<Func<TModel, object>> sourceProperty, TModel model) 
            where TModel : class, new()
        {
            sourceProperty.Body.GetNameAndType(out string propertyName, out Type type);

            var modelType = typeof(TModel);
            var property = modelType.GetPublicProperty(propertyName);
            var propertyValue = property.GetValue(model);
            
            string whereCondition = $"{propertyName} = ";

            if (type == typeof(bool)) whereCondition += $"{propertyValue}";
            else if (type == typeof(int)) whereCondition += $"{propertyValue}";
            else if (type == typeof(long)) whereCondition += $"{propertyValue}";
            else if (type == typeof(string)) whereCondition += $"'{propertyValue}'";

            return whereCondition;
        }
    }
}
