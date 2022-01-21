using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonProcessor
{
    internal class JsonObject
    {
        public Dictionary<string, object?> Members { get; set; } = new Dictionary<string, object?>();

        public int Level { get; set; }

        public override string ToString()
        {
            string output = "{";

            foreach (var member in Members) 
            {
                var memberValue = member.Value;

                if (memberValue is string)
                {
                    output += $"\"{member.Key}\":\"{memberValue}\"";
                }
                else if (memberValue is bool) 
                {
                    output += $"\"{member.Key}\":\"{memberValue.ToString()?.ToLower()}\"";
                }
                else
                {
                    output += $"\"{member.Key}\":{memberValue}";
                }

                if (member.Key != Members.Last().Key) 
                {
                    output += ",";
                }
            }

            output += "}";

            return output;
        }
    }
}
