using ImageService.Logger.Model;

namespace ImageService.Model
{
    public interface IImageServiceModel
    {
        string AddFile(string path, out MessageTypeEnum result);
    }
}
