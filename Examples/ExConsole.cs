using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Examples
{
    /// <summary>
    /// Simple console extension with "increased" multi-threading and color support
    /// (nothing but console visuals, no need to keep this code)
    /// </summary>
    public static class ExConsole
    {
        //
        private static List<ConsoleSegment> _buffer = new List<ConsoleSegment>();
        private static bool _writeBuffer;

        // Write
        public static void Write(string text)
        {
            Write(text, false);
        }
        public static void Write(string text, ConsoleColor color)
        {
            Write(text, color, false);
        }
        public static void Write(ConsoleSegment segment)
        {
            Write(segment, false);
        }
        public static void Write(ConsoleSegment[] segments)
        {
            Write(segments, false);
        }

        public static void Write(string text, bool write)
        {
            _buffer.Add(new ConsoleSegment(text));
            
            if (write)
                _writeBuffer = true;
        }
        public static void Write(string text, ConsoleColor color, bool write)
        {
            _buffer.Add(new ConsoleSegment(text, color));

            if (write)
                _writeBuffer = true;
        }
        public static void Write(ConsoleSegment segment, bool write)
        {
            _buffer.Add(segment);

            if (write)
                _writeBuffer = true;
        }
        public static void Write(ConsoleSegment[] segments, bool write)
        {
            _buffer.AddRange(segments);

            if (write)
                _writeBuffer = true;
        }

        // Write line(s)
        public static void WriteLine(string text)
        {
            WriteLine(text, false);
        }
        public static void WriteLine(string text, ConsoleColor color)
        {
            WriteLine(text, color, false);
        }
        public static void WriteLine(ConsoleSegment segment)
        {
            WriteLine(segment, false);
        }
        public static void WriteLines(ConsoleSegment[] segments)
        {
            WriteLines(segments, false);
        }

        public static void WriteLine(string text, bool write)
        {
            _buffer.Add(new ConsoleSegment(text + "\n"));

            if (write)
                _writeBuffer = true;
        }
        public static void WriteLine(string text, ConsoleColor color, bool write)
        {
            _buffer.Add(new ConsoleSegment(text + "\n", color));

            if (write)
                _writeBuffer = true;
        }
        public static void WriteLine(ConsoleSegment segment, bool write)
        {
            _buffer.Add(new ConsoleSegment(segment.Text + "\n", segment.Color));

            if (write)
                _writeBuffer = true;
        }
        public static void WriteLines(ConsoleSegment[] segments, bool write)
        {
            int length = segments.Length;
            for (int i = 0; i < length; i++)
                _buffer.Add(new ConsoleSegment(segments[i].Text + "\n", segments[i].Color));

            if (write)
                _writeBuffer = true;
        }

        // Buffer
        private static void WriteSegmentBuffer()
        {
            // Get count
            int length = _buffer.Count;

            // Abort if there are no segments
            if (length == 0)
                return;

            // Write segments to console
            for (int i = 0; i < length; i++)
                _buffer[i].Write();
            
            // Remove segments from buffer
            _buffer.RemoveRange(0, length);
        }

        public static void UpdateWriteBuffer()
        {
            // Checks if write buffer is ready
            if (_writeBuffer)
            {
                // Write buffer to console
                WriteSegmentBuffer();
                _writeBuffer = false;
            }
        }

        public static void ReadyWriteBuffer()
        {
            // Makes write buffer ready
            _writeBuffer = true;
        }

        // Cosnole Size
        public static int ConsoleWidth
        {
            get { return Console.BufferWidth; }
            set
            {
                Console.BufferWidth = value;
                Console.WindowWidth = value;
            }
        }
        public static int ConsoleHeight
        { get { return Console.BufferHeight; } set { Console.WindowHeight = value; } }
    }
}
