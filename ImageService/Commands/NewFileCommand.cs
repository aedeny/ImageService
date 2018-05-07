using ImageService.Model;
using Infrastructure.Logging;

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
            return _model.AddFile(args[0], out result);
        }
    }
}