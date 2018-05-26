using System.Diagnostics;

namespace ImageService.Model
{
    public interface IImageServiceModel
    {
        string AddFile(string path, out EventLogEntryType result);
    }
}