using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;
using TodoApp2.Entity.Extensions;
using TodoApp2.Entity.Model;

namespace TodoApp2.Entity.Query
{
    internal static class WhereBuilder
    {
        public static string EqualsWithModelExpression<TModel>(
            Expression<Func<TModel, object>> sourceProperty,
            TModel model)
            where TModel : EntityModel
        {
            sourceProperty.Body.GetNameAndType(out string propertyName, out Type type);

            var modelType = typeof(TModel);
            var property = modelType.GetPublicProperty(propertyName);
            var propertyValue = property.GetValue(model);

            string whereCondition = $"{propertyName} = {ValueToQueryValueHelper.FormatValue(propertyValue, type)}";
            return whereCondition;
        }

        public static string EqualsWithPrimaryKey<TModel>(TModel model, string primaryKeyName)
            where TModel : EntityModel
        {
            var modelType = typeof(TModel);
            var property = modelType.GetPublicProperty(primaryKeyName);
            var propertyValue = property.GetValue(model);
            var propertyType = propertyValue.GetType();

            string whereCondition = $"{primaryKeyName} = {ValueToQueryValueHelper.FormatValue(propertyValue, propertyType)}";
            return whereCondition;
        }

        public static string ExpressionToString<TModel>(
            Expression<Func<TModel, object>> whereExpression)
            where TModel : EntityModel
        {
            if (whereExpression == null) return null;

            var sb = new StringBuilder();

            try
            {
                AnalyzeExpression<TModel>(whereExpression.Body, sb);
            }
            catch (Exception ex)
            {
                Debugger.Break();
                throw;
            }

            return sb.ToString();
        }

        private static void AnalyzeExpression<TModel>(
            Expression expression,
            StringBuilder sb)
            where TModel : EntityModel
        {
            switch (expression)
            {
                case BinaryExpression be:
                    AnalyzeBinaryExpression<TModel>(be, sb);
                    break;
                case UnaryExpression ue:
                    if (expression.NodeType == ExpressionType.Not) sb.Append("(");
                    AnalyzeExpression<TModel>(ue.Operand, sb);
                    if (expression.NodeType == ExpressionType.Not) sb.Append(" = FALSE)");
                    break;
                case MemberExpression me:
                    var memberParentType = me.Expression.Type;
                    var modelType = typeof(TModel);

                    if (modelType != memberParentType)
                    {
                        try
                        {
                            var compiledExpression = Expression.Lambda(me).Compile();
                            object propertyValue = compiledExpression.DynamicInvoke();
                            var propertyType = propertyValue.GetType();

                            var formattedValue = ValueToQueryValueHelper.FormatValue(propertyValue, propertyType);
                            sb.Append(formattedValue);
                        }
                        catch (Exception)
                        {
                            throw new ArgumentException("Cannot parse expression!");
                        }
                    }
                    else
                    {
                        sb.Append(me.Member.Name);
                    }
                    break;
                case ConstantExpression ce:
                    sb.Append(ValueToQueryValueHelper.FormatValue(ce.Value, ce.Type));
                    break;
                default:
                    throw new ArgumentException("Cannot parse expression!");
            }
        }

        private static void AnalyzeBinaryExpression<TModel>(
            BinaryExpression expression,
            StringBuilder sb)
            where TModel : EntityModel
        {
            var nodeType = expression.NodeType;

            if (Comparisons.ContainsKey(nodeType))
            {
                var comparison = Comparisons[nodeType];

                sb.Append("(");

                AnalyzeExpression<TModel>(expression.Left, sb);
                sb.Append(comparison);
                AnalyzeExpression<TModel>(expression.Right, sb);

                sb.Append(")");
            }
            else
            {
                //AnalyzeExpression(expression.Left, sb);
                //AnalyzeExpression(expression.Right, sb);
                throw new ArgumentException("Cannot parse expression!");
            }
        }

        private static Dictionary<ExpressionType, string> Comparisons = new Dictionary<ExpressionType, string>
        {
            { ExpressionType.Equal, " = " },
            { ExpressionType.NotEqual, " != " },
            { ExpressionType.GreaterThan, " > " },
            { ExpressionType.GreaterThanOrEqual, " >= " },
            { ExpressionType.LessThan, " < " },
            { ExpressionType.LessThanOrEqual, " <= " },
            { ExpressionType.AndAlso , " AND " },
            { ExpressionType.OrElse , " OR " },
        };
    }
}
