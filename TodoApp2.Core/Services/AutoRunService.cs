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
        private bool _RunAtStartup;
        private string _BaseDirectory;
        private string _ExePath;

        private string BaseDirectory => _BaseDirectory ?? 
            (_BaseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

        private string ExePath => _ExePath ?? (_ExePath = Path.Combine(BaseDirectory, "TodoApp2.exe"));

        public bool RunAtStartup
        {
            get => _RunAtStartup;
            set
            {
                _RunAtStartup = value;

                if (_RunAtStartup)
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

            if (rk.GetValue(APP_NAME) == null)
            {
                rk.SetValue(APP_NAME, ExePath);
            }
        }
    }
}
