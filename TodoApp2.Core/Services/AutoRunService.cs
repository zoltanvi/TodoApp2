using Microsoft.Win32;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace TodoApp2.Core
{
    public class AutoRunService
    {
        private const string APP_NAME = "TodoApp2";
        private const string REGISTRY_PATH = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private bool _runAtStartup;
        private string _baseDirectory;
        private string _exePath;

        private string BaseDirectory => _baseDirectory ?? 
            (_baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

        private string ExePath => _exePath ?? (_exePath = Path.Combine(BaseDirectory, "TodoApp2.exe"));

        public bool RunAtStartup
        {
            get => _runAtStartup;
            set
            {
                _runAtStartup = value;

                if (_runAtStartup)
                {
                    SetRegistryKey();
                }
                else
                {
                    DeleteRegistryKey();
                }
            }
        }

        private void DeleteRegistryKey()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey(REGISTRY_PATH, true);

            if (rk != null)
            {
                rk.DeleteValue(APP_NAME, false);
                rk.Close();
            }
        }

        private void SetRegistryKey()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey(REGISTRY_PATH, true);
            string registryValue = rk.GetValue(APP_NAME) as string;
            if (registryValue == null || registryValue != ExePath)
            {
                rk.SetValue(APP_NAME, ExePath);
            }
        }
    }
}
