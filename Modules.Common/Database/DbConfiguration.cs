using System.Configuration;

namespace Modules.Common.Database
{
    public static class DbConfiguration
    {
        public static string DatabasePath { get; }
        public static string DatabasePathOld { get; }

        public static string ConnectionStringOld => $"Data Source={DatabasePathOld};";
        public static string ConnectionString => $"Data Source={DatabasePath};";

        static DbConfiguration()
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

            if (!IsFileNameValid(filenameWithExtension))
            {
                throw new ApplicationException($"{Constants.ConfigKeys.DatabaseFileName} is not a valid filename!");
            }

            DatabasePathOld = Path.Combine(directory, filenameWithExtension);
            DatabasePath = Path.Combine(directory, $"TodoApp_EF_core.{Constants.DatabaseFileExtension}");
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

        private static bool IsFileNameValid(string fileName)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char invalidChar in invalidChars)
            {
                if (fileName.Contains(invalidChar))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
