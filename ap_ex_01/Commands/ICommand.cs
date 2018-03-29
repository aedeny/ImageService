using ImageService.Infrastructure.Enums;
using ImageService.Logging.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands
{
    public interface ICommand
    {
        string Execute(string[] args, out MessageTypeEnum result);
    }
}
