using System;
using System.Configuration;
using System.IO;
using TodoApp2.Common;

namespace TodoApp2.Persistence
{
    public static class DataAccess
    {
        private static IAppContext Context;

        public static string DatabasePath { get; set; }
        //public static IAppContext GetAppContext() => Context ?? (Context = new AppContext($"Data Source={DatabasePath};"));

        static DataAccess()
        {
            string directory = ConfigurationManager.AppSettings[Constants.ConfigKeys.DatabaseDirectoryPath];
            string filename = ConfigurationManager.AppSettings[Constants.ConfigKeys.DatabaseFileName];
            string filenameWithExtension = $"{filename}.{Constants.DatabaseFileExtension}";

            if (string.IsNullOrWhiteSpace(directory))
            {
                directory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            }

            ValidatePath(directory);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!FileUtils.IsFileNameValid(filenameWithExtension))
            {
                throw new ApplicationException($"{Constants.ConfigKeys.DatabaseFileName} is not a valid filename!");
            }

            DatabasePath = Path.Combine(directory, filenameWithExtension);
        }

        private static void ValidatePath(string directory)
        {
            // Throws exception if the path is invalid
            Path.GetFullPath(directory);

            if (!Path.IsPathRooted(directory))
            {
                throw new ApplicationException($"{Constants.ConfigKeys.DatabaseDirectoryPath} must be a valid absolute path!");
            }
        }
    }
}
