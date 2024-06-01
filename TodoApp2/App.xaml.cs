using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modules.Common;
using Modules.Common.Database;
using Modules.Migrations;
using Modules.Notes.Repositories;
using Modules.Settings.Repositories;
using Modules.Settings.Services;
using Modules.Settings.ViewModels;
using System;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using TodoApp2.Core;
using TodoApp2.Persistence;
using TodoApp2.Services;
using TodoApp2.Services.Window;

namespace TodoApp2;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IHost _host;

    private const string PipeName = "TodoApp2Pipe";
    private const string ShowRunningAppWindowMessage = "ShowRunningApp";

    private static bool _isFirstInstance;
    private static Mutex _instanceMutex;

    private string m_CrashReportPath;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                ConfigureServices(services);
            })
            .Build();

        var migrationService = _host.Services.GetService<IMigrationService>();
        migrationService.Run();

        IoC.ServiceProvider = _host.Services;
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<MainWindow>();
        services.AddScoped<IAppContext, Persistence.AppContext>(provider =>
        {
            return new Persistence.AppContext(DbConfiguration.ConnectionStringOld);
        });
        services.AddScoped<IUIScaler, UIScaler>();
        services.AddScoped<AppViewModel>();
        services.AddScoped<MainWindowViewModel>();
        services.AddScoped<IWindowService, WindowService>();
        services.AddSingleton<IThemeManagerService, MaterialThemeManagerService>();
        services.AddSingleton<AppSettings>(provider =>
        {
            return AppSettings.Instance;
        });
        services.AddScoped<IAppSettingsService, AppSettingsService>();
        services.AddScoped<UndoManager>();
        services.AddScoped<MediaPlayerService>();
        services.AddScoped<ZoomingListener>();
        services.AddSingleton<UIScaler>(provider =>
        {
            return UIScaler.Instance;
        });
        services.AddSingleton<IUIScaler>(provider =>
        {
            return UIScaler.Instance;
        });
        services.AddScoped<ThemeChangeNotifier>();
        services.AddScoped<OverlayPageService>();

        services.AddScoped<CategoryListService>();
        services.AddScoped<TaskListService>();
        services.AddScoped<NoteListService>();
        services.AddScoped<TaskScheduler2>();
        services.AddScoped<ReminderNotificationService>();
        services.AddScoped<OneEditorOpenService>();

        AddDatabases(services);
    }

    private void AddDatabases(IServiceCollection services)
    {
        services.AddDbContext<SettingDbContext>();
        services.AddDbContext<NotesDbContext>();
        
        services.AddScoped<IMigrationService, MigrationService>();

        services.AddScoped<ISettingsRepository, SettingsRepository>();
        //services.AddScoped<INotesRepository, NotesRepository>();
        services.AddScoped<NotesRepository>();
    }

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

        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-EN");
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-EN");
        FrameworkElement.LanguageProperty.OverrideMetadata(
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

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

        // Load task content splitter service
        IoC.TaskContentSplitterService = TaskContentSplitterService.Instance;

        // Setup IoC. It can take some time
        IoC.Setup(_host.Services);

        // Load theme manager service
        //IThemeManagerService themeManagerService = MaterialThemeManagerService.Get(AppSettings.Instance);
        //IoC.ThemeManagerService = themeManagerService;

        // Show the main window
        //Current.MainWindow = new MainWindow();
        //Current.MainWindow.Show();
        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        var mainWindowViewModel = _host.Services.GetRequiredService<MainWindowViewModel>();
        mainWindow.DataContext = mainWindowViewModel;
        mainWindow.Show();
        Current.MainWindow = mainWindow;

        // The main window is open, so close the splash screen 
        splashScreen.Close(TimeSpan.Zero);

#if DEBUG
        //var devSettingsWindow = new DevSettingsWindow();
        var version = (string)Application.Current.TryFindResource(Constants.CurrentVersion);
        Application.Current.Resources[Constants.CurrentVersion] = $"{version}_Debug";
#endif
    }

    private SplashScreen ShowSplashScreenForTheme()
    {
        SplashScreen splashScreen = new SplashScreen($"Images/Splash.png");

#if !DEBUG
            splashScreen.Show(false, true);
#endif

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
                            if (Current.MainWindow.DataContext is MainWindowViewModel windowViewModel)
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
        StringBuilder sb = new StringBuilder();

        sb.Append(DateTime.Now.ToLongDateString() + "  ");
        sb.AppendLine(DateTime.Now.ToLongTimeString());
        sb.AppendLine(e.Exception.Message);
        sb.AppendLine();
        sb.AppendLine(e.Exception.StackTrace);

        var errorWindow = new ErrorWindow("An error occurred.", sb.ToString());
        errorWindow.ShowDialog();

        sb.AppendLine("======================================================");
        File.AppendAllText(m_CrashReportPath, sb.ToString());
    }
}
