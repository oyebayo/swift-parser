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
            Sections = new List<MessageSection>();
        }
        public List<MessageSection> Sections { get; set; }
        public override string ToString()
        {
            return string.Join("", Sections);
        }
    }
}
