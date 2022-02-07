using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JParser
{
    public static class JHelper
    {
        /// <summary>
        /// Removes all whitespace for a string. Helps with json parsing
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Formats a json string for easier viewing
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Allows user to add an additional json object a hierarchy for a more complex object
        /// </summary>
        /// <param name="originalJson"></param>
        /// <param name="newJson"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static object? AddToJson(object originalJson, object newJson, string query) 
        {
            //we need to be able to find the key and values of the new json to add to original
            //we need to be able to add inside the tree of the gameobject
            if (originalJson is JsonObject originalJsonObject && newJson is JsonObject newJsonObject) 
            {
                if (query == "root") 
                {
                    foreach (var member in newJsonObject.Members) 
                    {
                        originalJsonObject.Members.Add(member.Key, member.Value);
                    }
                    
                    return originalJsonObject;
                }

                Stack<JsonObject> stack = new Stack<JsonObject>();

                stack.Push(originalJsonObject);

                while (stack.Count > 0)
                {
                    JsonObject current = stack.Pop();

                    if (current.Members != null && current.Members.Any())
                    {
                        foreach (var member in current.Members)
                        {
                            if (member.Key.Equals(query, StringComparison.CurrentCultureIgnoreCase) && member.Value is JsonObject memberJsonObject)
                            {
                                foreach (var newMember in newJsonObject.Members) 
                                {
                                    memberJsonObject.Members.Add(newMember.Key, newMember.Value);
                                }

                                break;
                            }

                            if (member.Value is JsonObject jsonObject)
                            {
                                stack.Push(jsonObject);
                            }
                        }
                    }
                }

                return originalJson;
            }

            return null;
        }

        /// <summary>
        /// Allows the user to query a json object by key and return its value
        /// </summary>
        /// <param name="json"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static IEnumerable<object> Query(object json, string query) 
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

        /// <summary>
        /// Returns all the matching values in a query for a json array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="query"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns all the matching values in a query for a json object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        private static IEnumerable<object> Search(JsonObject obj, string query) 
        {
            List<object> results = new List<object>();

            Stack<JsonObject> stack = new Stack<JsonObject>();

            stack.Push(obj);

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
