using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FindComputerStuff.SwiftMessages
{
    public class MT103Parser
    {
        public List<MessageSection> Parse(string contents)
        {
            List<MessageSection> messages = new List<MessageSection>();
            MessageSection message = new MessageSection();
            messages.Add(message);

            int pos = 0;
            while (pos < contents.Length)
            {
                pos = ProcessCharacters(contents, pos, ref message);
                if (contents[pos] == '$')
                {
                    //we've reached a message boundary
                    message = new MessageSection();
                    messages.Add(message);
                    pos++;
                }
            }

            return messages;

        }
        public List<MessageSection> ParseFile(string file)
        {
            /*
                create messages array
                create new message
                do
                * if no more chars
                ** break out of loop
                * read char
                * is it a dollar sign? 
                ** close current message
                ** add to messages array
                ** set message = new message
                ** continue
                * is it a brace? 
                ** read chars until next colon
                ** use the characters found to create section
                ** read until next close brace to create 'content'
                ** split 'content' on LF and colon to create item array
                *** is it a single item array?
                **** assign value to the first item
                *** else
                ****for each item in array
                ***** read until next colon
                ***** use the xters except colon to create element
                ***** read until end to create value
                ***** close the element
                ** close the node
                loop
            */
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
            
            string[] fieldStrings = sn.Value.Split("\n:".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (fieldStrings.Length == 1)
                sn.Value = fieldStrings[0];
            else
            {
                foreach(string fieldString in fieldStrings)
                {
                    int separatorPos = fieldString.IndexOf(':');
                    Field f = new Field();
                    f.ID = fieldString.Substring(0, separatorPos);
                    f.Values = fieldString.Substring(0, separatorPos + 1).Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    sn.Fields.Add(f);
                }
                sn.Value = null;
            }

        }
    }
}
