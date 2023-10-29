using System.Collections.Generic;
using TodoApp2.Entity.Helpers;

namespace TodoApp2.Entity.Migration
{
    internal static class MigrationRunner
    {
        internal static int Run(List<DbMigration> migrations, int dbVersion, DbConnection connection)
        {
            int updatedDbVersion = dbVersion;

            for (int i = 0; i < migrations.Count; i++)
            {
                if (migrations[i].Version > dbVersion)
                {
                    updatedDbVersion = Run(migrations[i], connection);
                }
            }

            return updatedDbVersion;
        }

        internal static int Run(DbMigration dbMigration, DbConnection connection)
        {
            dbMigration.Up();
            var builder = dbMigration.MigrationBuilder;

            int addPropertyIndex = 0;
            int addModelIndex = 0;
            int removeModelIndex = 0;
            int customStepIndex = 0;

            for (int i = 0; i < builder.BuildSteps.Count; i++)
            {
                var buildStep = builder.BuildSteps[i];
                string query = null;

                if (buildStep == BuildStep.AddProperty)
                {
                    query = MigrationQueryBuilder.BuildAlterTable(builder.Properties[addPropertyIndex]);
                    addPropertyIndex++;
                }
                else if (buildStep == BuildStep.AddModel)
                {
                    query = ModelQueryBuilder.BuildCreateIfNotExists(builder.ModelBuilders[addModelIndex]);
                    addModelIndex++;
                }
                else if (buildStep == BuildStep.RemoveModel)
                {
                    query = ModelQueryBuilder.BuildDropTable(builder.ModelRemovers[removeModelIndex]);
                    removeModelIndex++;
                } 
                else if (buildStep == BuildStep.CustomStep)
                {
                    builder.CustomActions[customStepIndex]?.Invoke();
                    customStepIndex++;
                    continue;
                }

                QueryExecutionHelper.ExecuteQuery(connection, query);
            }

            return dbMigration.Version;
        }
    }
}
