using System.IO;
using System.Linq;

namespace TodoApp2.Common
{
    public static class FileUtils
    {
        public static bool IsFileNameValid(string fileName)
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
