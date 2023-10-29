using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TodoApp2.Common;
using TodoApp2.Entity.Extensions;
using TodoApp2.Entity.Info;
using TodoApp2.Entity.Model;

namespace TodoApp2.Entity.Migration
{
    public class MigrationBuilder
    {
        internal List<BuildStep> BuildSteps { get; } = new List<BuildStep>();
        internal List<PropInfo> Properties { get; } = new List<PropInfo>();
        internal List<BaseDbSetModelBuilder> ModelBuilders { get; } = new List<BaseDbSetModelBuilder>();
        internal List<string> ModelRemovers { get; } = new List<string>();
        internal List<Action> CustomActions { get; } = new List<Action>();

        internal MigrationBuilder()
        {
        }

        public MigrationBuilder AddProperty<TModel>(Expression<Func<TModel, object>> sourceProperty, string defaultValue = null)
            where TModel : class, new()
        {
            var info = new PropInfo();
            info.DefaultValue = defaultValue;

            info.ParentTypeName = typeof(TModel).Name;
            sourceProperty.Body.GetNameAndType(out info.PropName, out info.Type);

            Properties.Add(info);
            BuildSteps.Add(BuildStep.AddProperty);
            return this;
        }

        public DbSetModelBuilder<TModel> AddModel<TModel>(string modelName = null)
            where TModel : class, new()
        {
            var tableName = modelName ?? typeof(TModel).Name;
            var modelBuilder = new DbSetModelBuilder<TModel>(tableName);

            ModelBuilders.Add(modelBuilder);
            BuildSteps.Add(BuildStep.AddModel);

            return modelBuilder;
        }

        public void RemoveModel<TModel>()
            where TModel : class, new()
        {
            ModelRemovers.Add(typeof(TModel).Name);
            BuildSteps.Add(BuildStep.RemoveModel);
        }

        public void RemoveModel(string modelName)
        {
            modelName.ThrowIfEmpty();
            ModelRemovers.Add(modelName);
            BuildSteps.Add(BuildStep.RemoveModel);
        }

        public void ExecuteAction(Action action)
        {
            CustomActions.Add(action);
            BuildSteps.Add(BuildStep.CustomStep);
        }
    }
}
