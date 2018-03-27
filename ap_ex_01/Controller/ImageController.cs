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
        private Dictionary<CommandEnum, ICommand> mCommands;



        public ImageController(IImageServiceModel model)
        {
            mModel = model;
            mCommands = new Dictionary<CommandEnum, ICommand>()
            {
                {CommandEnum.NewFileCommand, new NewFileCommand(mModel)}
            };
        }
        public string ExecuteCommand(CommandEnum commandID, string[] args, out bool resultSuccesful)
        {
            resultSuccesful = false;

            if (!mCommands.TryGetValue(commandID, out ICommand currentCommand))
            {
                return "No such command";
            }

            Task<Tuple<string, bool>> task = new Task<Tuple<string, bool>>(() =>
            {
                string s = currentCommand.Execute(args, out bool temp);
                return Tuple.Create(s, temp);
            });
            task.Start();
            Tuple<string, bool> tuple = task.Result;
            resultSuccesful = tuple.Item2;
            return tuple.Item1;
        }
    }
}
