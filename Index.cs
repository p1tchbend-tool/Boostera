using System;

namespace Boostera
{
    public class Index
    {
        public string Path { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }

        public Index(string path, string text, DateTime timestamp) 
        {
            Path = path;
            Text = text;
            Timestamp = timestamp;
        }
    }
}
