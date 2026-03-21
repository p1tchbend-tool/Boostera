using System;

namespace Boostera
{
    internal class Index
    {
        internal string Path { get; set; }
        internal string Text { get; set; }
        internal DateTime Timestamp { get; set; }

        internal Index(string path, string text, DateTime timestamp) 
        {
            Path = path;
            Text = text;
            Timestamp = timestamp;
        }
    }
}
