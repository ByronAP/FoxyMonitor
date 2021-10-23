using FoxyMonitor.Contracts.Activation;
using FoxyMonitor.Contracts.Services;
using FoxyMonitor.Contracts.Views;
using FoxyMonitor.ViewModels;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FoxyMonitor.Services
{
    public class ApplicationHostService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly INavigationService _navigationService;
        private readonly IToastNotificationsService _toastNotificationsService;
        private readonly IApplicationPropertiesService _appPropertiesService;
        private readonly IThemeSelectorService _themeSelectorService;
        private readonly IEnumerable<IActivationHandler> _activationHandlers;
        private IShellWindow _shellWindow;
        private bool _isInitialized;

        public ApplicationHostService(IServiceProvider serviceProvider, IEnumerable<IActivationHandler> activationHandlers, INavigationService navigationService, IThemeSelectorService themeSelectorService, IApplicationPropertiesService appPropertiesService, IToastNotificationsService toastNotificationsService)
        {
            _serviceProvider = serviceProvider;
            _activationHandlers = activationHandlers;
            _navigationService = navigationService;
            _themeSelectorService = themeSelectorService;
            _appPropertiesService = appPropertiesService;
            _toastNotificationsService = toastNotificationsService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Initialize services that you need before app activation
            await InitializeAsync();

            await HandleActivationAsync();

            // Tasks after activation
            await StartupAsync();
            _isInitialized = true;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _appPropertiesService.PersistData();
            await Task.CompletedTask;
        }

        private async Task InitializeAsync()
        {
            if (!_isInitialized)
            {
                _appPropertiesService.RestoreData();
                _themeSelectorService.InitializeTheme();
                await Task.CompletedTask;
            }
        }

        private async Task StartupAsync()
        {
            if (!_isInitialized)
            {
                _toastNotificationsService.ShowToastNotificationSample();
                await Task.CompletedTask;
            }
        }

        private async Task HandleActivationAsync()
        {
            var activationHandler = _activationHandlers.FirstOrDefault(h => h.CanHandle());

            if (activationHandler != null)
            {
                await activationHandler.HandleAsync();
            }

            await Task.CompletedTask;

            if (!App.Current.Windows.OfType<IShellWindow>().Any())
            {
                // Default activation that navigates to the apps default page
                _shellWindow = _serviceProvider.GetService(typeof(IShellWindow)) as IShellWindow;
                _navigationService.Initialize(_shellWindow.GetNavigationFrame());
                _shellWindow.ShowWindow();
                _navigationService.NavigateTo(typeof(MainViewModel).FullName);
                await Task.CompletedTask;
            }
        }
    }
}
