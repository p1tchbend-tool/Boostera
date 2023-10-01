using System;

namespace Boostera
{
    public class MyListBoxItem
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime Timestamp { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public MyListBoxItem(string name, string path, DateTime timestamp)
        {
            Name = name;
            Path = path;
            Timestamp = timestamp;
        }
    }
}
