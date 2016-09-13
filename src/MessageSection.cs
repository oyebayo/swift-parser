using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FindComputerStuff.SwiftMessages
{
    public class MessageSection
    {
        public MessageSection()
        {
            Sections = new List<MessageSection>();
            Fields = new List<Field>();
            IsOpen = true;
        }
        public string ID { get; set; }
        public string Value { get; set; }
        public List<MessageSection> Sections { get; set; }
        public List<Field> Fields { get; set; }
        internal bool IsOpen { get; set; }
    }
}
