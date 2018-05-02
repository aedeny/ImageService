using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructer
{
    public class Enums
    {
        public enum CommandEnum : int
        {
            NewFileCommand,
            GetConfigCommand,
            LogCommand,
            CloseCommand
        }
    }
}
