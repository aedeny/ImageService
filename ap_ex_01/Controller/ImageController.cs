using ImageService.Commands;
using ImageService.Enums;
using ImageService.Logger.Model;
using ImageService.Model;
using System;
using System.Collections.Generic;
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
        public string ExecuteCommand(CommandEnum commandID, string[] args, out MessageTypeEnum result)
        {
            result = MessageTypeEnum.FAILURE;

            if (!mCommands.TryGetValue(commandID, out ICommand currentCommand))
            {
                return "No such command";
            }

            // Task == Thread
            Task<Tuple<string, MessageTypeEnum>> task = new Task<Tuple<string, MessageTypeEnum>>(() =>
            {
                string s = currentCommand.Execute(args, out MessageTypeEnum temp);
                return Tuple.Create(s, temp);
            });
            task.Start();
            Tuple<string, MessageTypeEnum> tuple = task.Result;
            result = tuple.Item2;
            return tuple.Item1;
        }
    }
}
