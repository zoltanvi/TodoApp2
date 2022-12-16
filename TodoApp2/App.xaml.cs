using System;
using System.IO;
using System.Windows;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private string m_CrashReportPath;

        /// <summary>
        /// Custom startup so we load our IoC immediately before anything else
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            SubscribeToExceptionHandling();

            // Let the base application do what it needs
            base.OnStartup(e);

            // Load async service
            IAsyncActionService asyncActionService = AsyncActionService.Instance;
            IoC.AsyncActionService = asyncActionService;

            // Setup IoC
            IoC.Setup();

            // Show the main window
            Current.MainWindow = new MainWindow();
            Current.MainWindow.Show();
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