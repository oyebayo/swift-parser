using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FindComputerStuff.SwiftMessages
{
    public class Field
    {
        public string ID { get; set; }
        public string Value
        {
            get
            {
                return (Values != null && Values.Length > 0) ? String.Join(Environment.NewLine, Values) : String.Empty;
            }
        }
        public string[] Values { get; set; }
    }
}
