using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oxylabs_BulkKeywords
{
    public class XmlSanitizingStream : StreamReader
    {
        public XmlSanitizingStream(Stream streamToSanitize)
        : base(streamToSanitize, true)
        { }

        /// <summary>
        /// Whether a given character is allowed by XML 1.0.
        /// </summary>
        public static bool IsLegalXmlChar(int character)
        {
            return
            (
                 character == 0x9 /* == '\t' == 9   */          ||
                 character == 0xA /* == '\n' == 10  */          ||
                 character == 0xD /* == '\r' == 13  */          ||
                (character >= 0x20 && character <= 0xD7FF) ||
                (character >= 0xE000 && character <= 0xFFFD) ||
                (character >= 0x10000 && character <= 0x10FFFF)
            );
        }
        private const int EOF = -1;

        public override int Read()
        {
            // Read each char, skipping ones XML has prohibited

            int nextCharacter;

            do
            {
                // Read a character

                if ((nextCharacter = base.Read()) == EOF)
                {
                    // If the char denotes end of file, stop
                    break;
                }
            }

            // Skip char if it's illegal, and try the next

            while (!XmlSanitizingStream.
                    IsLegalXmlChar(nextCharacter));

            return nextCharacter;
        }

        public override int Peek()
        {
            // Return next legal XML char w/o reading it 

            int nextCharacter;

            do
            {
                // See what the next character is 
                nextCharacter = base.Peek();
            }
            while
            (
                // If it's illegal, skip over 
                // and try the next.

                !XmlSanitizingStream
                .IsLegalXmlChar(nextCharacter) &&
                (nextCharacter = base.Read()) != EOF
            );

            return nextCharacter;

        }
    }
}
