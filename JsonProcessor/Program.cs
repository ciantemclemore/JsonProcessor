using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using JParser;
using FileHelper;

namespace JsonProcessor
{
    class Program
    {
        private static readonly JsonStorage JsonStorage = new JsonStorage();

        [STAThread]
        static void Main(string[] args)
        {            
            bool runProg = true;

            while (runProg)
            {
                int userSelection = DisplayMenu();

                switch (userSelection)
                {
                    case 1:
                        UploadJson();
                        break;
                    case 2:
                        object? jsonConent = SelectJsonContent();
                        if (jsonConent != null) 
                        {
                            DisplayContent(jsonConent);
                        }
                        break;
                    case 3:
                        UpdateJson();
                        break;
                    case 4:
                        QueryAJson();
                        break;
                    case 5:
                        runProg = false;
                        break;
                    default:
                        Console.WriteLine("Invalid selection. Try Again:");
                        break;
                }

                Console.WriteLine();
            }
        }

        private static int DisplayMenu()
        {
            Console.WriteLine("1. Select json to parse");
            Console.WriteLine("2. Display a parsed json");
            Console.WriteLine("3. Add to an existing json");
            Console.WriteLine("4. Query a json for a value");
            Console.WriteLine("5. Exit");

            int selectionValue;
            bool isValid = Int32.TryParse(Console.ReadLine(), out selectionValue);

            if (isValid)
            {
                return selectionValue;
            }

            //Just return an invalid selction to display menu again
            Console.WriteLine();
            return -1;
        }

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
                    JsonStorage.JsonStore.Add((result.Value.Item1, parsedJson));
                }
            }
        }

        private static object? SelectJsonContent() 
        {
            //This will provide space between the menu
            Console.WriteLine();

            int userSelection; 

            for (int i = 0; i < JsonStorage.JsonStore.Count; i++) 
            {
                Console.WriteLine($"{i+1}. {JsonStorage.JsonStore[i].Item1}");
            }

            Int32.TryParse(Console.ReadLine(), out userSelection);

            return JsonStorage.JsonStore[userSelection - 1].Item2;
        }

        private static void UpdateJson()
        {
            Console.WriteLine("Select a json to update: ");
            
            // first, allow the user to select the json they would like to update
            object? jsonContent = SelectJsonContent();

            if (jsonContent != null) 
            {
                
            }
        }

        private static void QueryAJson()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Select a json to query: ");

                //get the selected json the user wants to query
                object? jsonContent = SelectJsonContent();

                //disply the json content
                DisplayContent(new List<object>() { jsonContent });

                //allow the user to 'query' the object by 'key' and return a value
                Console.WriteLine("Enter 'query' to get value: (enter 'exit' to return)");
                string query = Console.ReadLine();

                if (query == "exit")
                {
                    break;
                }

                var queryResults = JHelper.Search(jsonContent, query);
                DisplayContent(queryResults);
            }
        }

        private static void DisplayContent(object content) 
        {
            //This will provide space between the menu
            Console.WriteLine();

            if (content != null) 
            {  
                Console.WriteLine(JHelper.Beautify(content.ToString()));   
            }
        }

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