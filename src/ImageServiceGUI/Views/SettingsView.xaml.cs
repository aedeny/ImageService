using ImageServiceGUI.ViewModels;

namespace ImageServiceGUI.Views
{
    /// <summary>
    ///     Interaction logic for LogView.xaml
    /// </summary>
    public partial class SettingsView
    {
        public SettingsView()
        {
            InitializeComponent();
            DataContext = new SettingsViewModel();
        }
    }
}