using FoxyMonitor.Contracts.Services;
using FoxyMonitor.Controls;
using FoxyMonitor.Models;
using FoxyMonitor.Properties;
using MahApps.Metro.Controls;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace FoxyMonitor.ViewModels
{
    public class ShellViewModel : ObservableObject
    {
        public HamburgerMenuItem SelectedMenuItem
        {
            get { return _selectedMenuItem; }
            set
            {
                SetProperty(ref _selectedMenuItem, value);

                if (value is CustomHamburgerMenuImageItem customMenuItem && customMenuItem.Tag != null)
                {
                    if (uint.TryParse(customMenuItem.Tag.ToString(), out var accountId))
                    {
                        _accountService.SelectedAccount = _accountService.Accounts.FirstOrDefault(x => x.Id == accountId);
                    }
                }
            }
        }

        public HamburgerMenuItem SelectedOptionsMenuItem
        {
            get { return _selectedOptionsMenuItem; }
            set { SetProperty(ref _selectedOptionsMenuItem, value); }
        }

        public ObservableCollection<HamburgerMenuItem> MenuItems { get; set; } = new ObservableCollection<HamburgerMenuItem>();

        public ObservableCollection<HamburgerMenuItem> OptionMenuItems { get; } = new ObservableCollection<HamburgerMenuItem>()
        {
            new HamburgerMenuGlyphItem() { Label = Resources.ShellAddAccountPage, Glyph = "\uE948", TargetPageType = typeof(AddAccountViewModel) },
            new HamburgerMenuGlyphItem() { Label = Resources.ShellSettingsPage, Glyph = "\uE713", TargetPageType = typeof(SettingsViewModel) }
        };

        public RelayCommand GoBackCommand => _goBackCommand ??= new RelayCommand(OnGoBack, CanGoBack);

        public ICommand MenuItemInvokedCommand => _menuItemInvokedCommand ??= new RelayCommand(OnMenuItemInvoked);

        public ICommand OptionsMenuItemInvokedCommand => _optionsMenuItemInvokedCommand ??= new RelayCommand(OnOptionsMenuItemInvoked);

        public ICommand LoadedCommand => _loadedCommand ??= new RelayCommand(OnLoaded);

        public ICommand UnloadedCommand => _unloadedCommand ??= new RelayCommand(OnUnloaded);

        private readonly INavigationService _navigationService;
        private readonly IAccountService _accountService;
        private HamburgerMenuItem _selectedMenuItem;
        private HamburgerMenuItem _selectedOptionsMenuItem;
        private RelayCommand _goBackCommand;
        private ICommand _menuItemInvokedCommand;
        private ICommand _optionsMenuItemInvokedCommand;
        private ICommand _loadedCommand;
        private ICommand _unloadedCommand;

        public ShellViewModel(IAccountService accountService, INavigationService navigationService)
        {
            _accountService = accountService;
            _navigationService = navigationService;

            _accountService.Accounts.CollectionChanged += Accounts_CollectionChanged;
        }

        private void Accounts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e == null) return;

            foreach (var account in _accountService.Accounts)
            {
                if (!MenuItems.Any(x => (uint)x.Tag == account.Id))
                {
                    var newMenuItem = BuildMenuItem(account);
                    MenuItems.Add(newMenuItem);
                }
                else
                {
                    var item = (CustomHamburgerMenuImageItem)MenuItems.First(x => (uint)x.Tag == account.Id);
                    if (item.Label != account.DisplayName) item.Label = account.DisplayName;
                    if (item.SubTitle != account.PoolName)
                    {
                        item.SubTitle = account.PoolName;
                        item.Thumbnail = new BitmapImage(new Uri($"pack://application:,,,/FoxyMonitor;component/Resources/{account.PoolName}.png"));
                    }
                }
            }

            foreach (var item in MenuItems)
            {
                if (!_accountService.Accounts.Any(x => x.Id == (uint)item.Tag))
                {
                    MenuItems.Remove(item);
                }
            }
        }

        private void LoadMenuItems()
        {
            OnPropertyChanging(nameof(MenuItems));

            var menuItems = new ObservableCollection<HamburgerMenuItem>();

            foreach (var account in _accountService.Accounts)
            {
                menuItems.Add(BuildMenuItem(account));
            }

            MenuItems = menuItems;

            OnPropertyChanged(nameof(MenuItems));
        }

        private static CustomHamburgerMenuImageItem BuildMenuItem(Account account)
        {
            return new CustomHamburgerMenuImageItem()
            {
                Tag = account.Id,
                Label = account.DisplayName,
                SubTitle = account.PoolName,
                Thumbnail = new BitmapImage(new Uri($"pack://application:,,,/FoxyMonitor;component/Resources/{account.PoolName}.png")),
                TargetPageType = typeof(MainViewModel)
            };
        }

        private void OnLoaded()
        {
            LoadMenuItems();
            _navigationService.Navigated += OnNavigated;
        }

        private void OnUnloaded()
        {
            _navigationService.Navigated -= OnNavigated;
        }

        private bool CanGoBack()
            => _navigationService.CanGoBack;

        private void OnGoBack()
            => _navigationService.GoBack();

        private void OnMenuItemInvoked()
            => NavigateTo(SelectedMenuItem.TargetPageType);

        private void OnOptionsMenuItemInvoked()
            => NavigateTo(SelectedOptionsMenuItem.TargetPageType);

        private void NavigateTo(Type targetViewModel)
        {
            if (targetViewModel != null)
            {
                _navigationService.NavigateTo(targetViewModel.FullName);
            }
        }

        private void OnNavigated(object sender, string viewModelName)
        {
            var item = MenuItems
                        .OfType<HamburgerMenuItem>()
                        .FirstOrDefault(i => viewModelName == i.TargetPageType?.FullName);
            if (item != null)
            {
                SelectedMenuItem = item;
            }
            else
            {
                SelectedOptionsMenuItem = OptionMenuItems
                        .OfType<HamburgerMenuItem>()
                        .FirstOrDefault(i => viewModelName == i.TargetPageType?.FullName);
            }

            GoBackCommand.NotifyCanExecuteChanged();
        }
    }
}
