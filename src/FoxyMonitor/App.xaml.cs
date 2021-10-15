using ControlzEx.Theming;
using FoxyMonitor.Data.Models;
using FoxyMonitor.Properties;
using FoxyMonitor.Services;
using FoxyMonitor.Utils;
using FoxyMonitor.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Windows;

namespace FoxyMonitor
{
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
    public partial class App : Application
    {
        internal static Thread MainThread;
        internal static IHost Host_Builder;
        private Mutex _appSingletonMutex;
        private EventWaitHandle _appEventWaitHandle;
        private Thread _appWaitHandleThread;

        public App()
        {
            if (!IsSingleton()) return;

            MainThread = Thread.CurrentThread;

            //Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            Host_Builder = new HostBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(Settings.Default.LogLevel);
                    if (Settings.Default.LogToFile)
                    {
                        logging.AddFile(options =>
                        {
                            options.IncludeScopes = true;
                            options.RootPath = IOUtils.GetLoggingDirectory();
                            options.CounterFormat = Constants.LogFileCounterFormat;
                            options.MaxFileSize = (long)Constants.MaxLogFileSize;
                            options.MaxQueueSize = (int)Constants.MaxLogWriteQueueSize;
                            options.Files = new[]
                            {
                                new Karambolo.Extensions.Logging.File.LogFileOptions
                                {
                                    Path = Constants.LogFileNameTemplate
                                }
                            };
                        });
                    }
                    else if (Settings.Default.ShowConsole)
                    {
                        ConsoleUtils.ShowConsole();
                        logging.AddConsole();
                    }
#if DEBUG
                    logging.AddDebug();
#endif
                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<AccountUpdaterService>();
                    services.AddHostedService<CheckForUpdatesService>();
                    services.AddHostedService<PoolInfoUpdaterService>();
                    services.AddSingleton<AppViewModel>();
                    services.AddSingleton<MainAppWindow>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            if (Host_Builder == null) return;

            await Host_Builder.StartAsync();

            SetTheme();

            base.OnStartup(e);

            var mainWindow = Host_Builder.Services.GetService<MainAppWindow>();
            mainWindow.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            //HACK: but we will flow with it

            base.OnExit(e);

            if (Host_Builder != null)
            {
                await Host_Builder.StopAsync();

                Host_Builder.Dispose();
            }

            try
            {
                _ = _appEventWaitHandle.Set();
                _appEventWaitHandle.Close();
                _appEventWaitHandle.Dispose();
            }
            catch
            {
                // IGNORE
            }

            Current.Shutdown();
        }

        public static void SetTheme()
        {
            switch (Settings.Default.ThemeMode)
            {
                default:
                case ThemeMode.Auto:
                    ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncAll;
                    ThemeManager.Current.SyncTheme();
                    break;
                case ThemeMode.Light:
                    _ = ThemeManager.Current.ChangeTheme(App.Current, $"Light.{Settings.Default.ThemeColor}");
                    break;
                case ThemeMode.Dark:
                    _ = ThemeManager.Current.ChangeTheme(App.Current, $"Dark.{Settings.Default.ThemeColor}");
                    break;
            }
        }

        private bool IsSingleton()
        {
            // only allow 1 instance to run at a time by using a mutex
            // if another instance is running a handle will fire off our event
            // to bring the running instance to the foreground
            _appSingletonMutex = new Mutex(true, Constants.AppMutexName, out var isOwned);
            _appEventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, Constants.AppId);

            GC.KeepAlive(_appSingletonMutex);

            if (isOwned)
            {
                // Spawn a thread which will be waiting for our event
                _appWaitHandleThread = new Thread(() =>
                {
                    try
                    {
                        // wait for our event to get fired
                        while (_appEventWaitHandle != null && !_appEventWaitHandle.SafeWaitHandle.IsClosed && _appEventWaitHandle.WaitOne())
                        {
                            _ = Dispatcher.BeginInvoke(new Action(() =>
                              {
                                  var mainAppWindow = (MainAppWindow)MainWindow;
                                  mainAppWindow.BringToForeground();

                              }));
                        }
                    }
                    catch
                    {
                        // ignore since this can throw in a miriad of ways
                    }
                })
                { IsBackground = true };

                _appWaitHandleThread.Start();
            }
            else
            {
                // Notify other instance so it could bring itself to foreground.
                _ = _appEventWaitHandle.Set();

                // Terminate this instance.
                OnExit(null);
                return false;
            }

            return true;
        }
    }
}
