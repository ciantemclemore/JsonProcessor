using System.Collections.Generic;
using System.Linq;

namespace JParser
{
    public class JsonArray
    {
        public List<object?> Elements { get; set; } = new List<object?>();

        public int Level { get; set; }

        public override string ToString()
        {
            string output = "[";

            foreach (var element in Elements)
            {

                if (element is string)
                {
                    if (element == "null")
                    {
                        output += $"null";
                    }
                    else 
                    {
                        output += $"\"{element}\"";
                    }
                }
                else if (element is bool)
                {
                    output += $"{element.ToString()?.ToLower()}";
                }
                else 
                {
                    output += $"{element.ToString()}";
                }

                if (element != Elements.Last())
                {
                    output += ",";
                }
            }

            output += "]";

            return output;
        }
    }
}
