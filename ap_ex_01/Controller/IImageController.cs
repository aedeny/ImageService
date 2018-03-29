using ImageService.Infrastructure.Enums;
using ImageService.Logging.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Controller
{
    public interface IImageController
    {
        string ExecuteCommand(CommandEnum commandID, string[] args, out MessageTypeEnum result); 
    }
}
