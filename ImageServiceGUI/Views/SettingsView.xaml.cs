using System.Windows.Controls;
using ImageServiceGUI.ViewModels;

namespace ImageServiceGUI.Views
{
    /// <summary>
    /// Interaction logic for LogView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
            DataContext = new SettingsViewModel();
        }

        //public string TabText
        //{
        //    get => (string) GetValue(TabTextProperty);
        //    set => SetValue(TabTextProperty, value);
        //}

        //public static readonly DependencyProperty TabTextProperty = DependencyProperty.Register("TabText",
        //    typeof(string), typeof(LogView), new PropertyMetadata("Default"));
    }
}