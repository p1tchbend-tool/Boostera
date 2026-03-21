using System;

namespace Boostera
{
    internal class MyListBoxItem
    {
        internal string Name { get; set; }
        internal string Path { get; set; }
        internal DateTime Timestamp { get; set; }

        public override string ToString()
        {
            return Name;
        }

        internal MyListBoxItem(string name, string path, DateTime timestamp)
        {
            Name = name;
            Path = path;
            Timestamp = timestamp;
        }
    }
}
