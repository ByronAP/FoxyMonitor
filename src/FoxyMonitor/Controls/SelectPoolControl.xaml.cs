using FoxyMonitor.Data.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace FoxyMonitor.Controls
{
    /// <summary>
    /// Interaction logic for SelectPoolControl.xaml
    /// </summary>
    public partial class SelectPoolControl : UserControl
    {
#pragma warning disable CS8603 // Possible null reference return.
        public static readonly DependencyProperty PoolTypeProperty =
            DependencyProperty.Register("PoolType", typeof(PoolType),
                typeof(UserInputControl), new PropertyMetadata(null));

        public static readonly DependencyProperty PoolNameProperty =
            DependencyProperty.Register("PoolName", typeof(string),
                typeof(UserInputControl), new PropertyMetadata(null));

        public PoolType PoolType { get => (PoolType)GetValue(PoolTypeProperty); set { SetValue(PoolTypeProperty, value); } }

        public string PoolName { get => GetValue(PoolNameProperty) as string; set { SetValue(PoolNameProperty, value); } }

        public event SelectedPoolTypeChanged? OnSelectedPoolTypeChanged;
        public event SelectedPoolNameChanged? OnSelectedPoolNameChanged;
#pragma warning restore CS8603 // Possible null reference return.

        public SelectPoolControl()
        {
            InitializeComponent();

            PoolType = PoolType.POST;
            PoolName = "chia_og";
        }

        private void PoolType_SplitButton_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PoolType_SplitButton == null || PoolName_SplitButton == null) return;

            if (PoolType_SplitButton.SelectedItem is PoolType selectedPoolType)
            {
                PoolType = selectedPoolType;

                switch (selectedPoolType)
                {
                    case PoolType.POST:
                        PoolName_SplitButton.ItemsSource = Enum.GetNames(typeof(FoxyPoolApi.PostPool));
                        PoolName_SplitButton.SelectedIndex = 1;
                        break;
                    case PoolType.POC:
                        PoolName_SplitButton.ItemsSource = Enum.GetNames(typeof(FoxyPoolApi.PocPool));
                        PoolName_SplitButton.SelectedIndex = 0;
                        break;
                }

                OnSelectedPoolTypeChanged?.Invoke(selectedPoolType);
            }
        }

        private void PoolName_SplitButton_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PoolType_SplitButton == null || PoolName_SplitButton == null || PoolName_SplitButton.SelectedItem == null || CoinLogo == null) return;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var poolName = PoolName_SplitButton.SelectedItem.ToString().ToLowerInvariant();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            PoolName = poolName;

            // Show the pool coin logo
            switch (poolName)
            {
                case "chia":
                    CoinLogo.Template = (ControlTemplate)Application.Current.Resources["ChiaNftGlyph"];
                    break;
                case "chia_og":
                    CoinLogo.Template = (ControlTemplate)Application.Current.Resources["ChiaGlyph"];
                    break;
                case "chives_og":
                    CoinLogo.Template = (ControlTemplate)Application.Current.Resources["ChivesGlyph"];
                    break;
                case "flax_og":
                    CoinLogo.Template = (ControlTemplate)Application.Current.Resources["FlaxGlyph"];
                    break;
                case "hddcoin_og":
                    CoinLogo.Template = (ControlTemplate)Application.Current.Resources["HddCoinGlyph"];
                    break;
            }

            OnSelectedPoolNameChanged?.Invoke(poolName);
        }
    }
}
