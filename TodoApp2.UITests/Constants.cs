using System;
using System.IO;

namespace TodoApp2.UITests
{
    public static class Constants
    {
        public const string ExeFileName = "TodoApp2.exe";
        public const string DatabaseName = "TodoApp2Database.db";
        public static string DatabasePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), DatabaseName);
        
        public static readonly TimeSpan HalfSec = new TimeSpan(0, 0, 0, 0, 500);
        public static readonly TimeSpan OneSec = new TimeSpan(0, 0, 1);
        public static readonly TimeSpan TwoSec = new TimeSpan(0, 0, 2);
        public static readonly TimeSpan ThreeSec = new TimeSpan(0, 0, 3);

        public const int VeryFastMouseSpeed = 1500;
        public const int FastMouseSpeed = 600;
        public const int NormalMouseSpeed = 300;
        public const int SlowMouseSpeed = 150;
    }
}
