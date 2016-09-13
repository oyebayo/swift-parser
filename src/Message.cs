using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FindComputerStuff.SwiftMessages
{
    public class Message
    {
        public Message()
        {
            Sections = new List<Block>();
        }
        public List<Block> Sections { get; set; }
        public override string ToString()
        {
            return string.Join("", Sections);
        }
    }
}
