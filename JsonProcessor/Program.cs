using FileHelper;
using JParser;
using System;
using System.Collections.Generic;

namespace JsonProcessor
{
    internal class Program
    {
        private static List<(string, object?)> JsonStorage = new List<(string, object?)>();

        [STAThread]
        private static void Main(string[] args)
        {
            bool runProgram = true;

            while (runProgram)
            {
                int userSelection = DisplayMenu();

                switch (userSelection)
                {
                    case 1:
                        UploadJson();
                        break;
                    case 2:
                        DisplayJson();
                        break;
                    case 3:
                        UpdateJson();
                        break;
                    case 4:
                        QueryAJson();
                        break;
                    case 5:
                        runProgram = false;
                        break;
                    default:
                        Console.WriteLine("Invalid selection. Try Again:");
                        break;
                }

                Console.WriteLine();
            }
        }

        /// <summary>
        /// Displays the main window options for the program
        /// </summary>
        /// <returns></returns>
        private static int DisplayMenu()
        {
            Console.WriteLine("1. Upload a json for parsing");
            Console.WriteLine("2. Display a parsed json");
            Console.WriteLine("3. Add to an existing json");
            Console.WriteLine("4. Query a json for a value");
            Console.WriteLine("5. Exit");

            bool isValid = int.TryParse(Console.ReadLine(), out int selectionValue);

            if (isValid)
            {
                return selectionValue;
            }

            Console.WriteLine();

            //Just return an invalid selction to display menu again
            return -1;
        }

        /// <summary>
        /// Allows the user to select a json file and have it parsed and converted into a c# object
        /// </summary>
        private static void UploadJson()
        {
            (string, string)? result = JFile.SelectFile();

            if (result != null)
            {
                JsonParser jsonParser = new JsonParser();
                object? parsedJson = jsonParser.Parse(result.Value.Item2);

                if (parsedJson != null)
                {
                    Console.WriteLine();
                    Console.WriteLine("Json Parsed and Stored successfully");
                    JsonStorage.Add((result.Value.Item1, parsedJson));
                }
            }
        }

        /// <summary>
        /// Displays a parsed json in a user friendly format
        /// </summary>
        private static void DisplayJson()
        {
            Console.WriteLine("Select a json to display: ");

            (string, object?) selectedJson = SelectJson();

            if (selectedJson.Item2 != null)
            {
                DisplayContent(selectedJson.Item2);
            }
        }

        /// <summary>
        /// Displays all of the parsed json's and allows a user to select one
        /// </summary>
        /// <returns>A ValueTuple(string, object?). The first item is the filename of the json and the second item is the parsed json.</returns>
        private static (string, object?) SelectJson()
        {
            Console.WriteLine();


            // Display all available json's stored in the JsonStorage
            for (int i = 0; i < JsonStorage.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {JsonStorage[i].Item1}");
            }

            int.TryParse(Console.ReadLine(), out int userSelection);

            return (JsonStorage[userSelection - 1].Item1, JsonStorage[userSelection - 1].Item2);
        }

        /// <summary>
        /// Allows the user to select a parsed json and add additional json to it. This helps create more complex objects
        /// </summary>
        private static void UpdateJson()
        {
            Console.WriteLine("Select a json to update: ");

            // first, allow the user to select the json they would like to update
            (string, object?) jsonContent = SelectJson();

            if (jsonContent.Item2 != null)
            {
                DisplayContent(jsonContent);
                Console.WriteLine();
                Console.WriteLine("Enter 'key' you would like to add json to: (enter 'exit' to return) ");
                string key = Console.ReadLine();

                if (key == "exit")
                {
                    return;
                }

                Console.WriteLine("Select new json: ");
                (string, string)? result = JFile.SelectFile();

                JsonParser jsonParser = new JsonParser();
                object? parsedJson = jsonParser.Parse(result.Value.Item2);

                if (parsedJson != null)
                {
                    object? updatedJson = JHelper.AddToJson(jsonContent.Item2, parsedJson, key);

                    if (updatedJson == null)
                    {
                        Console.WriteLine("Must provide valid json object: { 'key': 'value' }");
                    }
                    else
                    {
                        int index = JsonStorage.FindIndex(kvp => kvp.Item1 == jsonContent.Item1);

                        if (index != -1)
                        {
                            JsonStorage[index] = (jsonContent.Item1, updatedJson);
                            DisplayContent(updatedJson);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Allows the user to query an uploaded json by a 'key' within the json structure and return a value.
        /// </summary>
        private static void QueryAJson()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Select a json to query: ");

                //get the selected json the user wants to query
                object? jsonContent = SelectJson();

                //disply the json content
                DisplayContent(new List<object>() { jsonContent });

                //allow the user to 'query' the object by 'key' and return a value
                Console.WriteLine("Enter 'query' to get value: (enter 'exit' to return)");
                string query = Console.ReadLine();

                if (query == "exit")
                {
                    break;
                }

                IEnumerable<object>? queryResults = JHelper.Query(jsonContent, query);
                DisplayContent(queryResults);
            }
        }

        /// <summary>
        /// Displays a single json content in a user friendly manner.
        /// </summary>
        /// <param name="content"></param>
        private static void DisplayContent(object content)
        {
            //This will provide space between the menu
            Console.WriteLine();

            if (content != null)
            {
                Console.WriteLine(JHelper.Beautify(content.ToString()));
            }
        }

        /// <summary>
        /// Displays multiple json contents in a user friendly manner.
        /// </summary>
        /// <param name="contents"></param>
        private static void DisplayContent(IEnumerable<object> contents)
        {
            //This will provide space between the menu
            Console.WriteLine();

            if (contents != null)
            {
                foreach (object item in contents)
                {
                    Console.WriteLine(JHelper.Beautify(item.ToString()));
                }
            }
        }
    }
}