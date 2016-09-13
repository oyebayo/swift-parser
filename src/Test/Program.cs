using FindComputerStuff.SwiftMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            MT103Parser parser = new MT103Parser();
            var messages = parser.ParseFile(@"C:\Users\oyekoleb\Desktop\swift MT103\NEW\00122477.txt");
            Console.ReadKey();
        }
    }
}
