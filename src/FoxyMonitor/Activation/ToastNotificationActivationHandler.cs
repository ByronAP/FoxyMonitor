using FoxyMonitor.Contracts.Activation;
using FoxyMonitor.Contracts.Services;
using FoxyMonitor.Contracts.Views;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FoxyMonitor.Activation
{
    // For more information about sending a local toast notification from C# apps, see
    // https://docs.microsoft.com/windows/uwp/design/shell/tiles-and-notifications/send-local-toast?tabs=desktop
    // and https://github.com/microsoft/WindowsTemplateStudio/blob/release/docs/WPF/features/toast-notifications.md
    public class ToastNotificationActivationHandler : IActivationHandler
    {
        public const string ActivationArguments = "ToastNotificationActivationArguments";

        private readonly IConfiguration _config;
        private readonly IServiceProvider _serviceProvider;
        private readonly INavigationService _navigationService;

        public ToastNotificationActivationHandler(IConfiguration config, IServiceProvider serviceProvider, INavigationService navigationService)
        {
            _config = config;
            _serviceProvider = serviceProvider;
            _navigationService = navigationService;
        }

        public bool CanHandle()
        {
            return !string.IsNullOrWhiteSpace(_config[ActivationArguments]);
        }

        public async Task HandleAsync()
        {
            if (!App.Current.Windows.OfType<IShellWindow>().Any())
            {
                // Here you can get an instance of the ShellWindow and choose navigate
                // to a specific page depending on the toast notification arguments
            }
            else
            {
                App.Current.MainWindow.Activate();
                if (App.Current.MainWindow.WindowState == WindowState.Minimized)
                {
                    App.Current.MainWindow.WindowState = WindowState.Normal;
                }
            }

            await Task.CompletedTask;
        }
    }
}
