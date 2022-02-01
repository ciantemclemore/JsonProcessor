using System;
using System.IO;
using System.Windows.Forms;

namespace FileHelper
{
    public static class JFile
    {
        /// <summary>
        /// Returns a file name and the file content
        /// </summary>
        /// <returns>(string, string)</returns>
        public static (string, string)? SelectFile()
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

                    return (fileName, content);
                }

                return null;
            }
        }

    }
}
