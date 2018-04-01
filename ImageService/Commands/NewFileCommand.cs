using ImageService.Logger.Model;
using ImageService.Model;

namespace ImageService.Commands
{
    public class NewFileCommand : ICommand
    {
        private readonly IImageServiceModel _model;

        public NewFileCommand(IImageServiceModel model)
        {
            _model = model;
        }

        public string Execute(string[] args, out MessageTypeEnum result)
        {
            // The string will return the new path if result is true, or the error message otherwise.
            return _model.AddFile(args[0], out result);
        }
    }
}
