using FoxyMonitor.Data.Models;
using FoxyPoolApi;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using static FoxyMonitor.Utils.ValidationRules;

namespace FoxyMonitor.Controls
{
    /// <summary>
    /// Interaction logic for AddAccountControl.xaml
    /// </summary>
    public partial class AddAccountControl : UserControl
    {
        private MainAppWindow? ParentWindow;

        public AddAccountControl()
        {
            InitializeComponent();
        }

        private void SelectPoolControl_OnSelectedPoolNameChanged(string poolName)
        {
        }

        private async void SelectPoolControl_OnSelectedPoolTypeChanged(PoolType poolType)
        {
            if (poolType == PoolType.POC)
            {
                _ = await DialogManager.ShowMessageAsync(ParentWindow, "Sorry", "Sorry, POC pools are not supported yet!", MessageDialogStyle.Affirmative);
            }
        }

        private async void SaveAccount_Click(object sender, RoutedEventArgs e)
        {
            var progressDialog = await ParentWindow.ShowProgressAsync("Please wait...", "Saving Account", false);

            try
            {
                var poolType = SelectedPool_Control.PoolType;
                var poolName = SelectedPool_Control.PoolName;

                var displayName = DisplayName_Input.Value;
                var launcherId = LauncherId_Input.Value;
                var authToken = AuthToken_Input.Value;

                if (poolType == PoolType.POST)
                {
                    using var postApiClient = new PostApiClient((PostPool)Enum.Parse(typeof(PostPool), poolName, true));
                    var accountResult = await postApiClient.GetAccountAsync(launcherId);
                    if (accountResult != null && !string.IsNullOrEmpty(accountResult.PayoutAddress))
                    {
                        if (authToken.Trim() != string.Empty)
                        {
                            var updateAccountNameResult = await postApiClient.UpdateAccountNameAsync(displayName, launcherId, authToken);
                            if (updateAccountNameResult != null)
                            {
                                if (updateAccountNameResult.StatusCode != System.Net.HttpStatusCode.NoContent)
                                {
                                    AuthToken_Input.Value = string.Empty;
                                    AuthToken_Input.ErrorText = "Invalid";
                                    AuthToken_Input.ErrorVisibility = Visibility.Visible;
                                    return;
                                }
                            }
                        }

                        // were ok so lets create a new account record
                        var newAccount = new Account
                        {
                            LauncherId = launcherId,
                            DisplayName = displayName,
                            AuthToken = authToken,
                            PoolType = poolType,
                            PoolName = poolName,
                            LastAcceptedPartialTime = accountResult.LastAcceptedPartialAt,
                            Collateral = accountResult.Collateral == null ? 0 : decimal.Parse(accountResult.Collateral),
                            Difficulty = accountResult.Difficulty,
                            DistributionRatio = accountResult.DistributionRatio == null ? "0-100" : accountResult.DistributionRatio,
                            EstCapacity = accountResult.Ec,
                            PendingBalance = accountResult.Pending == null ? 0 : decimal.Parse(accountResult.Pending),
                            PayoutAddress = accountResult.PayoutAddress == null ? "UNKNOWN" : accountResult.PayoutAddress,
                            PoolPubKey = accountResult.PoolPublicKey == null ? "UNKNOWN" : accountResult.PoolPublicKey,
                            Shares = accountResult.Shares,
                            LastBlockWon = 0,
                            LastUpdated = DateTimeOffset.UtcNow
                        };

                        try
                        {
                            var historicalResult = await postApiClient.GetAccountHistoricalAsync(launcherId);
                            var historicalItemsList = new ObservableCollection<PostAccountHistoricalDbItem>();
                            foreach (var item in historicalResult)
                                historicalItemsList.Add(PostAccountHistoricalDbItem.FromApiItem(item));

                            newAccount.PostAccountHistoricalDbItems = historicalItemsList;
                        }
                        catch (Exception ex)
                        {
                            ParentWindow?.Logger.LogError(ex, "Failed to create new account historical entries, possible api failure.");
                        }

                        try
                        {
                            if (ParentWindow == null)
                            {
                                // nothing we can do, can't even log it since parentwindow has the logger
                                return;
                            }
                            await ParentWindow.MainAppViewModel.AppViewDispatcher.InvokeAsync(async () =>
                            {
                                var fmDbContext = ParentWindow.MainAppViewModel.FmDbContext;

                                fmDbContext.Accounts.Add(newAccount);
                                await fmDbContext.SaveChangesAsync();
                            });

                            TestLauncherId_Button.IsEnabled = false;
                            DisplayName_Input.IsEnabled = false;
                            AuthToken_Input.IsEnabled = false;
                            SaveAccount_Button.IsEnabled = false;

                            LauncherId_Input.Value = string.Empty;
                            DisplayName_Input.Value = string.Empty;
                            AuthToken_Input.Value = string.Empty;

                            ParentWindow.AddAccountFlyout.IsOpen = false;
                            ParentWindow.AccountsFlyout.IsOpen = true;

                            //ParentWindow.MainAppViewModel.RaiseAccountsPropertyChanged();
                        }
                        catch (Exception ex)
                        {
                            ParentWindow?.Logger.LogError(ex, "Failed to save new account to db.");
                        }
                        finally
                        {
                            ParentWindow?.Logger.LogInformation("Created new account {Id}", newAccount.Id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ParentWindow?.Logger.LogError(ex, "Failed to create new account instance, possible api failure.");

                TestLauncherId_Button.IsEnabled = false;
                DisplayName_Input.IsEnabled = false;
                AuthToken_Input.IsEnabled = false;
                SaveAccount_Button.IsEnabled = false;

                LauncherId_Input.Value = string.Empty;
                DisplayName_Input.Value = string.Empty;
                AuthToken_Input.Value = string.Empty;
            }
            finally
            {
                await progressDialog.CloseAsync();
            }
        }

        private async void TestLauncherId_Click(object sender, RoutedEventArgs e)
        {
            if (ParentWindow == null)
            {
                // nothing we can do, can't even log it since parentwindow has the logger
                return;
            }

            var progressDialog = await ParentWindow.ShowProgressAsync("Please wait...", "Validating Launcher Id", false);

            try
            {
                var poolType = SelectedPool_Control.PoolType;
                var poolName = SelectedPool_Control.PoolName;
                var launcherId = LauncherId_Input.Value;

                if (poolType == PoolType.POST)
                {
                    using var postApiClient = new PostApiClient((PostPool)Enum.Parse(typeof(PostPool), poolName, true));
                    var accountResult = await postApiClient.GetAccountAsync(launcherId);
                    if (accountResult != null && !string.IsNullOrEmpty(accountResult.PayoutAddress))
                    {
                        if (!string.IsNullOrEmpty(accountResult.Name))
                        {
                            DisplayName_Input.Value = accountResult.Name;
                        }

                        DisplayName_Input.IsEnabled = true;
                        AuthToken_Input.IsEnabled = true;
                        SaveAccount_Button.IsEnabled = true;

                    }
                    else
                    {
                        DisplayName_Input.IsEnabled = false;
                        AuthToken_Input.IsEnabled = false;
                        SaveAccount_Button.IsEnabled = false;

                        DisplayName_Input.Value = string.Empty;
                        AuthToken_Input.Value = string.Empty;

                        LauncherId_Input.ErrorText = "Launcher Id Not Found";
                        LauncherId_Input.ErrorVisibility = Visibility.Visible;
                    }
                }
            }
            catch (Exception ex)
            {
                ParentWindow?.Logger.LogError(ex, "Failed to validate Launcher Id");
                DisplayName_Input.IsEnabled = false;
                AuthToken_Input.IsEnabled = false;
                SaveAccount_Button.IsEnabled = false;

                DisplayName_Input.Value = string.Empty;
                AuthToken_Input.Value = string.Empty;
            }
            finally
            {
                try
                {
                    await progressDialog.CloseAsync();
                }
                catch
                {
                    // ignore
                }
            }
        }

        private void LauncherId_Input_OnValueChanged(string value)
        {
            TestLauncherId_Button.IsEnabled = false;
            LauncherId_Input.ErrorVisibility = Visibility.Collapsed;

            DisplayName_Input.IsEnabled = false;
            AuthToken_Input.IsEnabled = false;
            SaveAccount_Button.IsEnabled = false;

            if (string.IsNullOrEmpty(value)) return;
            if (string.IsNullOrWhiteSpace(value)) return;

            // validate the launcher id
            var launcherIdValidator = new LauncherIDValidationRule();
            var validationResult = launcherIdValidator.Validate(value, CultureInfo.InvariantCulture);
            if (!validationResult.IsValid)
            {
                if (validationResult.ErrorContent != null)
                {
#pragma warning disable CS8601 // Possible null reference assignment.
                    LauncherId_Input.ErrorText = validationResult.ErrorContent.ToString();
#pragma warning restore CS8601 // Possible null reference assignment.
                    LauncherId_Input.ErrorVisibility = Visibility.Visible;
                }
            }
            else
            {
                TestLauncherId_Button.IsEnabled = true;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ParentWindow = (MainAppWindow)Window.GetWindow(this);
        }
    }
}