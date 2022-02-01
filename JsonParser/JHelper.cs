using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JParser
{
    public static class JHelper
    {
        public static string RemoveAllWhiteSpace(string json)
        {
            StringBuilder stringBuilder = new();
            bool isInsideString = false;

            for (int i = 0; i < json.Length; i++)
            {
                if (!char.IsWhiteSpace(json[i]))
                {
                    stringBuilder.Append(json[i]);

                    if (json[i] == '"' && (i == 0 || (i > 0 && json[i - 1] != '\\')))
                    {
                        isInsideString = !isInsideString;
                    }
                }
                else if (char.IsWhiteSpace(json[i]) && isInsideString)
                {
                    stringBuilder.Append(json[i]);
                }
            }

            return stringBuilder.ToString();
        }

        public static string Beautify(string json)
        {
            int level = 0;
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < json.Length; i++)
            {
                if (json[i] == '{' || json[i] == '[')
                {
                    stringBuilder.Append(json[i]);
                    stringBuilder.Append("\n");

                    level++;

                    stringBuilder.Append(' ', 4 * level);
                }
                else if (json[i] == '}' || json[i] == ']')
                {
                    stringBuilder.Append("\n");
                    level--;
                    stringBuilder.Append(' ', 4 * level);
                    stringBuilder.Append(json[i]);
                }
                else if (json[i] == ':')
                {
                    stringBuilder.Append(json[i]);
                    stringBuilder.Append(' ');
                }
                else if ((json[i] == ',') && (json[i + 1] == '"' || json[i+1] == '{')) 
                {
                    stringBuilder.Append(json[i]);
                    stringBuilder.Append("\n");
                    stringBuilder.Append(' ', 4 * level);
                }
                else
                {
                    stringBuilder.Append(json[i]);
                }
            }

            return stringBuilder.ToString();
        }

        public static IEnumerable<object> Search(object json, string query) 
        {
            if (json is JsonObject jsonObject)
            {
                return Search(jsonObject, query);
            }
            else if (json is JsonArray jsonArray)
            {
                return Search(jsonArray, query);
            }

            return new List<object>();
        }

        private static IEnumerable<object> Search(JsonArray array, string query) 
        {
            List<object> results = new List<object>();

            foreach (var element in array.Elements) 
            {
                if (element is JsonObject jsonObject)
                {
                    return Search(jsonObject, query);
                }
                else
                {
                    results.Add(element);
                }
            }

            return results;
        }

        private static IEnumerable<object> Search(JsonObject root, string query) 
        {
            List<object> results = new List<object>();

            Stack<JsonObject> stack = new Stack<JsonObject>();

            stack.Push(root);

            while (stack.Count > 0) 
            {
                JsonObject current = stack.Pop();

                if (current.Members != null && current.Members.Any()) 
                {
                    foreach (var member in current.Members) 
                    {
                        if (member.Key.Equals(query, StringComparison.CurrentCultureIgnoreCase)) 
                        {
                            results.Add(member.Value);
                        }

                        if (member.Value is JsonObject jsonObject) 
                        {
                            stack.Push(jsonObject);
                        }
                    }
                }
            }

            return results;
        }
    }
}
