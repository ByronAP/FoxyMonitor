using System.Windows;
using System.Windows.Controls;

namespace FoxyMonitor.Controls
{
    /// <summary>
    /// Interaction logic for UserInputControl.xaml
    /// </summary>
    public partial class UserInputControl : UserControl
    {
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string),
                typeof(UserInputControl), new PropertyMetadata(null));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string),
                typeof(UserInputControl), new PropertyMetadata(null));

        public static readonly DependencyProperty ErrorTextProperty =
            DependencyProperty.Register("ErrorText", typeof(string),
                typeof(UserInputControl), new PropertyMetadata(null));

        public static readonly DependencyProperty ErrorVisibilityProperty =
            DependencyProperty.Register("ErrorVisibility", typeof(Visibility),
                typeof(UserInputControl), new PropertyMetadata(null));

        public string Label { get => GetValue(LabelProperty) as string; set { SetValue(LabelProperty, value); } }
        public string Value
        {
            get => GetValue(ValueProperty) as string;
            set
            {
                SetValue(ValueProperty, value);
                OnValueChanged?.Invoke(Value);
            }
        }
        public string ErrorText { get => GetValue(ErrorTextProperty) as string; set { SetValue(ErrorTextProperty, value); } }
        public Visibility ErrorVisibility { get => (Visibility)GetValue(ErrorVisibilityProperty); set { SetValue(ErrorVisibilityProperty, value); } }

        public event ValueChanged OnValueChanged;

        public UserInputControl()
        {
            ErrorVisibility = Visibility.Collapsed;
            InitializeComponent();
            DataContext = this;
        }

        private void Input_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Value = Input_TextBox.Text;
            OnValueChanged?.Invoke(Value);
        }
    }
}
