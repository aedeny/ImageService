using ImageService.Infrastructure;
using ImageService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands
{
    public class NewFileCommand : ICommand
    {
        private IImageServiceModel mModel;

        public NewFileCommand(IImageServiceModel Model)
        {
            mModel = Model;
        }

        public string Execute(string[] args, out bool result)
        {
            // The string will return the new path if result is true, or the error message otherwise.
            return mModel.AddFile(args[0], out result);
        }
    }
}
