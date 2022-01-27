using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using JParser;

namespace JsonProcessor
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // Create a new JsonStorage for tracking all our parsed jsons
            JsonStorage jsonStorage = new JsonStorage();
            
            bool runProg = true;

            while (runProg)
            {
                int userSelection = DisplayMenu();

                switch (userSelection)
                {
                    case 1:
                        (string, string)? result = SelectFile();

                        if (result != null) 
                        {
                            string cleanJson = JsonExtension.RemoveAllWhiteSpace(result.Value.Item2);
                            
                            JsonParser jsonParser = new JsonParser();
                            
                            object? parsedJson = jsonParser.Parse(cleanJson);

                            if (parsedJson != null) 
                            {
                                Console.WriteLine();
                                Console.WriteLine("Json Parsed and Stored successfully");
                                Console.WriteLine();
                                jsonStorage.JsonStore.Add((result.Value.Item1, parsedJson));
                            }
                        }
                        break;
                    case 2:
                        object? jsonConent = SelectJsonContent(jsonStorage);
                        DisplayContent(jsonConent);
                        break;
                    case 3:
                        
                        break;
                    case 4:
                        
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

        private static (string, string)? SelectFile() 
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog()) 
            {
                openFileDialog.InitialDirectory = "C:\\";
                openFileDialog.Filter = "json files (*.json)|*.json|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 0;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK) 
                {
                    string fileName = openFileDialog.FileName;
                    var fileStrream = openFileDialog.OpenFile();
                    string content = string.Empty;

                    using (StreamReader sr = new StreamReader(fileStrream)) 
                    {
                        content = sr.ReadToEnd();    
                    }

                    return (fileName,content);
                }

                return null;
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

        private static object? SelectJsonContent(JsonStorage jsonStorage) 
        {
            //This will provide space between the menu
            Console.WriteLine();

            int userSelection; 

            for (int i = 0; i < jsonStorage.JsonStore.Count; i++) 
            {
                Console.WriteLine($"{i+1}. {jsonStorage.JsonStore[i].Item1}");
            }

            Int32.TryParse(Console.ReadLine(), out userSelection);

            return jsonStorage.JsonStore[userSelection - 1].Item2;
        }

        private static void DisplayContent(object? content) 
        {
            //This will provide space between the menu
            Console.WriteLine();

            if (content != null) 
            {
                Console.WriteLine(JsonExtension.Beautify(content.ToString()));
            }
        }
    }
}