using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace JParser
{
    public class JsonParser
    {
        private int position = 0;

        public JsonParser() 
        {
        }

        public object? Parse(string value) 
        {
            string cleanJson = JHelper.RemoveAllWhiteSpace(value);
            return Parse(cleanJson, 0);
        }

        private object? Parse(string value, int level = 0)
        {
            if (value[position] == 't')
            {
                return ParseTrue(value);
            }
            else if (value[position] == 'f')
            {
                return ParseFalse(value);
            }
            else if (value[position] == 'n')
            {
                return ParseNull(value);
            }
            else if (char.IsDigit(value[position]) || value[position] == '-')
            {
                return ParseNumber(value);
            }
            else if (value[position] == '"')
            {
                return ParseString(value);
            }
            else if (value[position] == '[')
            {
                return ParseJsonArray(value, level + 1);
            }
            else if (value[position] == '{')
            {
                return ParseJsonObject(value, level + 1);
            }
            else
            {
                return null;
            }
        }

        string ParseString(string value)
        {
            //go pass the original quote 
            position++;

            int startPosition = position;

            //go until the pointer reaches the end of the string
            for (int i = position; i < value.Length; i++)
            {
                if (value[i] == '"' && value[i - 1] != '\\')
                {
                    break;
                }
                else
                {
                    position++;
                }
            }

            return Regex.Unescape(value.Substring(startPosition, (position) - startPosition));
        }

        object? ParseNumber(string value)
        {
            int startPosition = position;

            if (value[position] == '-')
            {
                position += 1;
            }

            //checking for integers
            ParseDigits(value);

            //check for fractions
            if (position < value.Length && value[position] == '.')
            {
                position += 1;
                ParseDigits(value);
            }

            //check for exponenets
            if (position < value.Length && (value[position] == 'e' || value[position] == 'E'))
            {
                position += 1;

                if (value[position] == '+' || value[position] == '-')
                {
                    position += 1;
                    ParseDigits(value);
                }
            }

            decimal number = decimal.Parse(value.Substring(startPosition, position - startPosition), NumberStyles.Float, CultureInfo.CreateSpecificCulture("en-US"));

            //puts us back on the last digit since numbers don't have quotes in them
            position--;

            return number;
        }


        void ParseDigits(string value)
        {
            for (int i = position; i < value.Length; i++)
            {
                if (char.IsDigit(value[i]))
                {
                    position++;
                }
                else
                {
                    break;
                }
            }
        }

        bool ParseTrue(string json)
        {
            string trueString = json.Substring(position, 4);

            if (trueString != "true")
            {
                throw new Exception("Invalid Json");
            }

            position += 3;

            return true;
        }

        bool ParseFalse(string json)
        {
            string falseString = json.Substring(position, 5);

            if (falseString != "false")
            {
                throw new Exception("Invalid Json");
            }

            position += 4;

            return false;
        }

        object? ParseNull(string json)
        {
            string nullString = json.Substring(position, 4);

            if (nullString != "null")
            {
                throw new Exception("Invalid Json");
            }

            position += 3;

            return null;
        }

        JsonArray ParseJsonArray(string json, int level = 0)
        {

            //starts us at the quote and not the curly brace
            position++;

            JsonArray jsonArray = new JsonArray() { Level = level };

            for (; position < json.Length; position++)
            {
                if (json[position] == ']')
                {
                    return jsonArray;
                }
                else if (json[position] == ',')
                {
                    continue;
                }
                else
                {
                    jsonArray.Elements.Add(Parse(json, level));
                }
            }
            throw new Exception("Invalid Json");
        }

        JsonObject ParseJsonObject(string json, int level = 0)
        {
            //starts us at the quote and not the curly brace
            position++;

            JsonObject jsonObject = new JsonObject() { Level = level };

            for (; position < json.Length; position++)
            {
                if (json[position] == '"')
                {
                    //create key
                    string key = ParseString(json);

                    //skip the colon and go to value
                    position += 2;

                    jsonObject.Members.Add(key, Parse(json, level));

                }
                else if (json[position] == '}')
                {
                    return jsonObject;
                }
                else if (json[position] == ',')
                {
                    // do nothing, we want to go to the next character
                    // remove from code after further testing
                    continue;
                }
                else
                {
                    throw new Exception("Invalid Json");
                }
            }
            throw new Exception("Invalid Json");
        }
    }
}
