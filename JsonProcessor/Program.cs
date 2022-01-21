// See https://aka.ms/new-console-template for more information
using JsonProcessor;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;


string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), "json*.json");
int position;

//foreach (var file in files) 
//{
//}
position = 0;
string jsonString = File.ReadAllText(files[0]);
string? obj = RemoveAllWhiteSpace(jsonString);
object? newObj = ParseValue(obj);
Console.WriteLine(newObj);


string RemoveAllWhiteSpace(string json)
{
    StringBuilder stringBuilder = new StringBuilder();
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
            jsonArray.Elements.Add(ParseValue(json, level));
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

            jsonObject.Members.Add(key, ParseValue(json, level));

        }
        else if (json[position] == '}')
        {
            return jsonObject;
        }
        else if (json[position] == ',')
        {
            continue;
        }
        else
        {
            throw new Exception("Invalid Json");
        }
    }
    throw new Exception("Invalid Json");
}


object? ParseValue(string value, int level = 0)
{
    if (value[position] == 't')
    {
        return ParseTrue(value);
    }
    else if (value[position] == 'f')
    {
        return ParseFalse(value); ;
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