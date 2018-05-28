using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;

namespace ImageServiceGUI.Model
{
    internal interface ISettingsModel
    {
        string OutputDirectory { get; }
        int ThumbnailSize { get; }
        string LogName { get; }
        string SourceName { get; }
        SolidColorBrush BackgroundColor { get; }
        ObservableCollection<string> DirectoryHandlers { get; set; }
        void OnRemove(string selectedDirectoryHandler);
        event PropertyChangedEventHandler PropertyChanged;
    }
}