using System;
using System.Linq.Expressions;

namespace TodoApp2.Entity.Extensions
{
    internal static class ExpressionExtensions
    {
        public static void GetNameAndType(this Expression ex, out string name, out Type type)
        {
            if (ex is MemberExpression memberExpression)
            {
                name = memberExpression.Member.Name;
                type = memberExpression.Type;
            }
            else if (ex is UnaryExpression unaryExpression &&
                unaryExpression.Operand is MemberExpression innerMemberExpression)
            {
                // Handle cases where the expression is wrapped in a unary expression (e.g., type casts)
                name = innerMemberExpression.Member.Name;
                type = innerMemberExpression.Type;
            }
            else
            {
                throw new ArgumentException("Cannot process property!");
            }
        }
    }
}
