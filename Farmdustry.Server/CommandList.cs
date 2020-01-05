using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmdustry.Server
{
    public class CommandList
    {
        List<byte> commandsToSend = new List<byte>();

        public void Clear()
        {
            commandsToSend = new List<byte>();
        }

        public void Add(byte[] command)
        {
            commandsToSend.AddRange(command);
        }

        public byte[] ToByteArray()
        {
            return commandsToSend.ToArray();
        }
    }
}
