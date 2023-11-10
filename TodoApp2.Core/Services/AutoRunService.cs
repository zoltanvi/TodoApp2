using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace TodoApp2.Core
{
    /// <summary>
    /// Manages the automatic execution of the application at Windows startup 
    /// by creating or removing a shortcut in the Windows Startup folder.
    /// </summary>
    public class AutoRunService
    {
        private static string StartupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

        private bool _runAtStartup;
        private string _baseDirectory;
        private string _exePath;

        private string BaseDirectory => _baseDirectory ?? (_baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        private string ExePath => _exePath ?? (_exePath = Path.Combine(BaseDirectory, "TodoApp2.exe"));
        private string ShortcutPath => Path.Combine(StartupFolderPath, "TodoApp2.lnk");

        /// <summary>
        /// A value indicating whether the application should run at Windows startup.
        /// </summary>
        public bool RunAtStartup
        {
            get => _runAtStartup;
            set
            {
                _runAtStartup = value;

                if (_runAtStartup)
                {
                    AddShortcut();
                }
                else
                {
                    RemoveShortcut();
                }
            }
        }

        /// <summary>
        /// Removes the shortcut from the Windows Startup folder if it exists.
        /// </summary>
        private void RemoveShortcut()
        {
            if (File.Exists(ShortcutPath))
            {
                File.Delete(ShortcutPath);
            }
        }

        /// <summary>
        /// Adds a shortcut to the Windows Startup folder for the application.
        /// </summary>
        public void AddShortcut()
        {
            if (!File.Exists(ShortcutPath))
            {
                IShellLink link = (IShellLink)new ShellLink();

                // setup shortcut information
                link.SetPath(ExePath);

                // save it
                IPersistFile file = (IPersistFile)link;
                file.Save(ShortcutPath, false);
            }
        }

        [ComImport]
        [Guid("00021401-0000-0000-C000-000000000046")]
        internal class ShellLink
        {
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("000214F9-0000-0000-C000-000000000046")]
        internal interface IShellLink
        {
            void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, out IntPtr pfd, int fFlags);
            void GetIDList(out IntPtr ppidl);
            void SetIDList(IntPtr pidl);
            void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);
            void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
            void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);
            void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
            void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);
            void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
            void GetHotkey(out short pwHotkey);
            void SetHotkey(short wHotkey);
            void GetShowCmd(out int piShowCmd);
            void SetShowCmd(int iShowCmd);
            void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);
            void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
            void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);
            void Resolve(IntPtr hwnd, int fFlags);
            void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
        }
    }
}
