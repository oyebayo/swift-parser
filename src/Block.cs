using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FindComputerStuff.SwiftMessages
{
    public class Block
    {
        public Block()
        {
            Sections = new List<Block>();
            Fields = new List<Field>();
            IsOpen = true;
        }
        public string ID { get; set; }
        public string Value { get; set; }
        public List<Block> Sections { get; set; }
        public List<Field> Fields { get; set; }
        internal bool IsOpen { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{{{0}:", ID);
            if (Sections != null && Sections.Any()) sb.Append(string.Join("", Sections.Select(s => s.ToString()).ToArray()));
            if (Fields != null && Fields.Any())
            {
                sb.Append(Environment.NewLine);
                sb.Append(string.Join(Environment.NewLine, Fields.Select(f => f.ToString()).ToArray()));
            }
            sb.AppendFormat("{0}",Value);
            sb.Append("}");
            return sb.ToString();
        }
    }
}
