using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace JParser
{
    public class JsonParser
    {
        private int position = 0;

        public JsonParser()
        {
        }

        public object? Parse(string json)
        {
            string cleanJson = JHelper.RemoveAllWhiteSpace(json);
            return Parse(cleanJson, 0);
        }

        private object? Parse(string json, int level = 0)
        {
            if (json[position] == 't')
            {
                return ParseTrue(json);
            }
            else if (json[position] == 'f')
            {
                return ParseFalse(json);
            }
            else if (json[position] == 'n')
            {
                return ParseNull(json);
            }
            else if (char.IsDigit(json[position]) || json[position] == '-')
            {
                return ParseNumber(json);
            }
            else if (json[position] == '"')
            {
                return ParseString(json);
            }
            else if (json[position] == '[')
            {
                return ParseJsonArray(json, level + 1);
            }
            else if (json[position] == '{')
            {
                return ParseJsonObject(json, level + 1);
            }
            else
            {
                return null;
            }
        }

        private string ParseString(string json)
        {
            //go pass the original quote 
            position++;

            int startPosition = position;

            //go until the pointer reaches the end of the string
            for (int i = position; i < json.Length; i++)
            {
                if (json[i] == '"' && json[i - 1] != '\\')
                {
                    break;
                }
                else
                {
                    position++;
                }
            }

            return Regex.Unescape(json.Substring(startPosition, (position) - startPosition));
        }

        private object? ParseNumber(string json)
        {
            int startPosition = position;

            if (json[position] == '-')
            {
                position += 1;
            }

            //checking for integers
            ParseDigits(json);

            //check for fractions
            if (position < json.Length && json[position] == '.')
            {
                position += 1;
                ParseDigits(json);
            }

            //check for exponenets
            if (position < json.Length && (json[position] == 'e' || json[position] == 'E'))
            {
                position += 1;

                if (json[position] == '+' || json[position] == '-')
                {
                    position += 1;
                    ParseDigits(json);
                }
            }

            decimal number = decimal.Parse(json.Substring(startPosition, position - startPosition), NumberStyles.Float, CultureInfo.CreateSpecificCulture("en-US"));

            //puts us back on the last digit since numbers don't have quotes in them
            position--;

            return number;
        }


        private void ParseDigits(string json)
        {
            for (int i = position; i < json.Length; i++)
            {
                if (char.IsDigit(json[i]))
                {
                    position++;
                }
                else
                {
                    break;
                }
            }
        }

        private bool ParseTrue(string json)
        {
            string trueString = json.Substring(position, 4);

            if (trueString != "true")
            {
                throw new ArgumentException("Invalid Json");
            }

            position += 3;

            return true;
        }

        private bool ParseFalse(string json)
        {
            string falseString = json.Substring(position, 5);

            if (falseString != "false")
            {
                throw new ArgumentException("Invalid Json");
            }

            position += 4;

            return false;
        }

        private object ParseNull(string json)
        {
            string nullString = json.Substring(position, 4);

            if (nullString != "null")
            {
                throw new ArgumentException("Invalid Json");
            }

            position += 3;

            return nullString;
        }

        private JsonArray ParseJsonArray(string json, int level = 0)
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
            throw new ArgumentException("Invalid Json");
        }

        private JsonObject ParseJsonObject(string json, int level = 0)
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
                    throw new ArgumentException("Invalid Json");
                }
            }
            throw new ArgumentException("Invalid Json");
        }
    }
}
