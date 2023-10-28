using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace TodoApp2.Entity
{
    internal struct PropInfo
    {
        public string Name;
        public Type Type;
        public bool IsPrimaryKey;
        public string DefaultValue;
    }

    internal struct ForeignKeyInfo
    {
        public string Name;
        public string ReferencedTableName;
        public string ReferencedPropertyName;
    }

    public class DbSetConfiguration<TModel> where TModel : class, new()
    {
        private DbSet<TModel> _dbSet;
        internal List<PropInfo> Properties { get; } = new List<PropInfo>();
        internal List<ForeignKeyInfo> ForeignKeys { get; } = new List<ForeignKeyInfo>();

        internal DbSetConfiguration(DbSet<TModel> dbSet)
        {
        }

        public DbSetConfiguration<TModel> Property(Expression<Func<TModel, object>> sourceProperty)
        {
            Property(sourceProperty, false, null);
            return this;
        }

        public DbSetConfiguration<TModel> Property(Expression<Func<TModel, object>> sourceProperty, bool isPrimaryKey)
        {
            Property(sourceProperty, isPrimaryKey, null);
            return this;
        }

        public DbSetConfiguration<TModel> Property(Expression<Func<TModel, object>> sourceProperty, string defaultValue)
        {
            Property(sourceProperty, false, defaultValue);
            return this;
        }

        public DbSetConfiguration<TModel> Property(Expression<Func<TModel, object>> sourceProperty, bool isPrimaryKey, string defaultValue)
        {
            var info = new PropInfo();
            info.IsPrimaryKey = isPrimaryKey;
            info.DefaultValue = defaultValue;

            GetNameAndType(sourceProperty.Body, out info.Name, out info.Type);

            Properties.Add(info);

            return this;
        }

        public DbSetConfiguration<TModel> ForeignKey<TReferenced>(
            Expression<Func<TModel, object>> sourceProperty,
            Expression<Func<TReferenced, object>> referencedProperty)
        {
            var info = new ForeignKeyInfo();

            GetNameAndType(sourceProperty.Body, out info.Name, out _);
            GetNameAndType(referencedProperty.Body, out info.ReferencedPropertyName, out _);
            info.ReferencedTableName = typeof(TReferenced).Name;

            ForeignKeys.Add(info);

            return this;
        }

        private static void GetNameAndType(Expression ex, out string name, out Type type)
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
