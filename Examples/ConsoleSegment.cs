using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Examples
{
    /// <summary>
    /// A segment of writable text (for the ExConsole)
    /// (nothing but console visuals, no need to keep this code)
    /// </summary>
    public struct ConsoleSegment
    {
        static readonly ConsoleColor DefaultColor = ConsoleColor.Gray;

        /// <summary>
        /// The text the segment will write
        /// </summary>
        public string Text;
        /// <summary>
        /// The color the segment will be written in
        /// </summary>
        public ConsoleColor Color;

        // Constructor(s)
        public ConsoleSegment(string text)
        {
            Text = text;
            Color = DefaultColor;
        }
        public ConsoleSegment(string text, ConsoleColor color)
        {
            Text = text;
            Color = color;
        }

        /// <summary>
        /// Writes the text (in the assigned color) to the console
        /// </summary>
        public void Write()
        {
            // Set console color
            Console.ForegroundColor = Color;

            // Write text
            Console.Write(Text);
        }
    }
}
