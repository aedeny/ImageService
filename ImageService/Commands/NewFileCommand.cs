using System.Diagnostics;
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

        public string Execute(string[] args, out EventLogEntryType result)
        {
            return _model.AddFile(args[0], out result);
        }
    }
}