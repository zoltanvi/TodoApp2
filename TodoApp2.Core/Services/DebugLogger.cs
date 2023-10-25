using System.Text;

namespace TodoApp2.Core
{
    public static class DebugLogger
    {
        private static StringBuilder _stringBuilder = new StringBuilder();

        public static string LogHistory => _stringBuilder.ToString();

        public static void Log(string message)
        {
#if DEBUG
            _stringBuilder.AppendLine(message);
#endif
        }
    }
}
