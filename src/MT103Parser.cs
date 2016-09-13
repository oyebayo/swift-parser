using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FindComputerStuff.SwiftMessages
{
    public class MT103Parser
    {
        public List<Message> Parse(string contents)
        {
            List<Message> messages = new List<Message>();
            MessageSection root = new MessageSection();
            int pos = 0;
            while (pos < contents.Length)
            {
                pos = ProcessCharacters(contents, pos, ref root);
                bool isEndOfMessage = (contents.Length - 3 >= pos && contents.Substring(pos, 3) == "{1:");
                bool isEndOfFile = (pos >= contents.Length);
                if(isEndOfFile || isEndOfMessage)
                {
                    Message message = new Message();
                    message.Sections.AddRange(root.Sections.ToArray());
                    messages.Add(message);
                    root.Sections.Clear();
                }
            }

            return messages;

        }
        public List<Message> ParseFile(string file)
        {
            string contents = File.ReadAllText(file, new System.Text.UTF8Encoding(false));
            return Parse(contents);
        }

        private int ProcessCharacters(string contents, int pos, ref MessageSection msg)
        {
            switch (contents[pos])
            {
                case '{':
                    MessageSection section = new MessageSection();//start a subsection
                    msg.Sections.Add(section);
                    pos++;
                    break;
                case '}':
                    if (!msg.Sections.Any(s => s.IsOpen))
                        throw new Exception("Closing brace without prior open brace, at position" + pos);

                    MessageSection sn = msg.Sections.Last(s => s.IsOpen);
                    sn.IsOpen = false;
                    GenerateFields(sn);
                    pos++;
                    break;
                case '$':
                    pos++; //skip
                    break;
                default:
                    if (!msg.Sections.Any(s => s.IsOpen))
                        throw new Exception("Characters without brace, at position " + pos);

                    MessageSection current = msg.Sections.Last(s => s.IsOpen);
                    int nextBoundary = contents.IndexOf(':', pos);
                    if (nextBoundary < -1) throw new Exception("Field delimiter not found, after position " + pos);
                    current.ID = contents.Substring(pos, nextBoundary - pos);
                    pos = nextBoundary + 1; //skip the colon

                    //is the next character a brace?
                    if (contents[pos] == '{')
                    {
                        while (pos < contents.Length)
                        {
                            pos = ProcessCharacters(contents, pos, ref current);
                            if (contents[pos] == '}' & current.Sections.Any(s => s.IsOpen) == false) break;
                        }
                    }
                    else
                    {
                        nextBoundary = contents.IndexOf('}', pos);
                        if (nextBoundary < -1) throw new Exception("Closing brace not found, after position " + pos);
                        current.Value = contents.Substring(pos, nextBoundary - pos);
                        pos = nextBoundary;
                    }

                    break;
            }

            return pos;
        }

        private void GenerateFields(MessageSection sn)
        {
            if (string.IsNullOrWhiteSpace(sn.Value)) return;
            
            string[] fieldStrings = sn.Value.Replace("\r", "").Split(new string[]{"\n:"}, StringSplitOptions.RemoveEmptyEntries);
            if (fieldStrings.Length == 1)
                sn.Value = fieldStrings[0];
            else
            {
                foreach(string fieldString in fieldStrings)
                {
                    int separatorPos = fieldString.IndexOf(':');
                    Field f = new Field();
                    f.ID = fieldString.Substring(0, separatorPos);
                    f.Values = fieldString.Substring(separatorPos + 1).Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    sn.Fields.Add(f);
                }
                sn.Value = null;
            }

        }
    }
}
