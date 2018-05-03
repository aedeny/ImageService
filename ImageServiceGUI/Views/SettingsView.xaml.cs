using System.Diagnostics;
using System.Windows.Controls;
using ImageServiceGUI.ViewModels;

namespace ImageServiceGUI.Views
{
    /// <summary>
    /// Interaction logic for LogView.xaml
    /// </summary>
    public partial class SettingsView
    {
        public SettingsView()
        {
            Debug.WriteLine("SettingView");
            InitializeComponent();
            DataContext = new SettingsViewModel();
        }
    }
}