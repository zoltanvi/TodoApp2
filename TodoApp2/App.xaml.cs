using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string PipeName = "TodoApp2Pipe";
        private const string ShowRunningAppWindowMessage = "ShowRunningApp";

        private static bool _isFirstInstance;
        private static Mutex _instanceMutex;

        private string m_CrashReportPath;

        /// <summary>
        /// Custom startup so we load our IoC immediately before anything else
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            SubscribeToExceptionHandling();

            _instanceMutex = new Mutex(true, PipeName, out _isFirstInstance);

            if (!_isFirstInstance)
            {
                SendMessageToRunningInstance(ShowRunningAppWindowMessage);
                Current.Shutdown();
                return;
            }

            // Setup the essential services and modules for the application
            IoC.PreSetup();

            SplashScreen splashScreen = ShowSplashScreenForTheme();

            // Let the base application do what it needs
            base.OnStartup(e);

            // Start the named pipe server to listen for messages from other instances
            Task.Run(() => ListenForMessages());

            // Load async service
            IAsyncActionService asyncActionService = AsyncActionService.Instance;
            IoC.AsyncActionService = asyncActionService;
            
            // Setup IoC. It can take some time
            IoC.Setup();

            // Load theme manager service
            IThemeManagerService themeManagerService = ThemeManagerService.Get(IoC.AppSettings);
            IoC.ThemeManagerService = themeManagerService;

            // Show the main window
            Current.MainWindow = new MainWindow();
            Current.MainWindow.Show();

            // The main window is open, so close the splash screen 
            splashScreen.Close(TimeSpan.Zero);
        }

        private SplashScreen ShowSplashScreenForTheme()
        {
            //List<Setting> settings = IoC.Database.GetSettings();
            //Setting activeThemeSetting = settings.FirstOrDefault(s => s.Key == nameof(ApplicationSettings.ActiveTheme));

            string activeThemeName = nameof(Theme.ExtraDark);

            SplashScreen splashScreen = new SplashScreen($"Images/Splash/{activeThemeName}.png");
            splashScreen.Show(false, true);
            
            return splashScreen;
        }

        private static void SendMessageToRunningInstance(string message)
        {
            using (var client = new NamedPipeClientStream(PipeName))
            {
                client.Connect();
                using (var writer = new StreamWriter(client))
                {
                    writer.WriteLine(message);
                    writer.Flush();
                }
            }
        }

        private static void ListenForMessages()
        {
            while (true)
            {
                using (var server = new NamedPipeServerStream(PipeName))
                {
                    server.WaitForConnection();
                    using (var reader = new StreamReader(server))
                    {
                        string message = reader.ReadLine();
                        if (message == ShowRunningAppWindowMessage)
                        {
                            Current.Dispatcher.Invoke(() =>
                            {
                                if (Current.MainWindow.DataContext is WindowViewModel windowViewModel)
                                {
                                    windowViewModel.ShowWindowRequested();
                                }
                            });
                        }
                    }
                }
            }
        }

        private void SubscribeToExceptionHandling()
        {
            string appDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string crashReportFileName = "TodoApp2_CrashReport.txt";
            m_CrashReportPath = Path.Combine(appDataFolderPath, crashReportFileName);

            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        // TODO: Implement more sophisticated logging
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string message = $"\n\n{DateTime.Now.ToLongDateString()}\n" +
                $"{DateTime.Now.ToLongTimeString()}\n" +
                $"{e.Exception.Message}\n\n" +
                $"{e.Exception.StackTrace}\n===========================";
            File.AppendAllText(m_CrashReportPath, message);
        }
    }
}