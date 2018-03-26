using ImageService.Commands;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Controller
{
    public class ImageController : IImageController
    {
        private IImageServiceModel mModel;
        private Dictionary<int, ICommand> mCommands;
        


        public ImageController(IImageServiceModel model)
        {
            mModel = model;
            mCommands = new Dictionary<int, ICommand>()
            {
                {0, new NewFileCommand(mModel)}
            };
        }
        public string ExecuteCommand(int commandID, string[] args, out bool resultSuccesful)
        {
            resultSuccesful = false;

            if (!mCommands.TryGetValue(commandID, out ICommand currentCommand))
            {
                return "No such command";
            }

            return currentCommand.Execute(args, out resultSuccesful);
        }
    }
}
