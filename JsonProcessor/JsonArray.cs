using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonProcessor
{
    internal class JsonArray
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
                    output += $"\"{element}\"";
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
