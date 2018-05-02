using Infrastructure.Logging;

namespace ImageService.Model
{
    public interface IImageServiceModel
    {
        string AddFile(string path, out MessageTypeEnum result);
    }
}