using System.Collections.Generic;

namespace JsonProcessor
{
    internal class JsonStorage
    {
        public List<(string, object?)> JsonStore { get; set; } = new();

        public string CurrentFileName { get; set; } = string.Empty;
    }
}
