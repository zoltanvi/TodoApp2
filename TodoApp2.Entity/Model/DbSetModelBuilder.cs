using System;
using System.Linq.Expressions;
using TodoApp2.Common;
using TodoApp2.Entity.Extensions;
using TodoApp2.Entity.Info;

namespace TodoApp2.Entity.Model
{
    public class DbSetModelBuilder<TModel> : BaseDbSetModelBuilder
        where TModel : EntityModel
    {
        internal override string TableName { get; }

        internal DbSetModelBuilder(string tableName)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(tableName);

            TableName = tableName;
        }

        public DbSetModelBuilder<TModel> Property(Expression<Func<TModel, object>> sourceProperty)
        {
            Property(sourceProperty, false, null);
            return this;
        }

        public DbSetModelBuilder<TModel> Property(Expression<Func<TModel, object>> sourceProperty, bool isPrimaryKey)
        {
            Property(sourceProperty, isPrimaryKey, null);
            return this;
        }

        public DbSetModelBuilder<TModel> Property(Expression<Func<TModel, object>> sourceProperty, string defaultValue)
        {
            Property(sourceProperty, false, defaultValue);
            return this;
        }

        public DbSetModelBuilder<TModel> Property(Expression<Func<TModel, object>> sourceProperty, bool isPrimaryKey, string defaultValue)
        {
            var info = new PropInfo();
            info.IsPrimaryKey = isPrimaryKey;
            info.DefaultValue = defaultValue;

            sourceProperty.Body.GetNameAndType(out info.PropName, out info.Type);

            Properties.Add(info);

            return this;
        }

        public DbSetModelBuilder<TModel> ForeignKey<TReferenced>(
            Expression<Func<TModel, object>> sourceProperty,
            Expression<Func<TReferenced, object>> referencedProperty)
        {
            var info = new ForeignKeyInfo();

            sourceProperty.Body.GetNameAndType(out info.Name, out _);
            referencedProperty.Body.GetNameAndType(out info.ReferencedPropertyName, out _);

            info.ReferencedTableName = typeof(TReferenced).Name;

            ForeignKeys.Add(info);

            return this;
        }
    }
}
