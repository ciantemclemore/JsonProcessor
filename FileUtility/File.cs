using System.IO;
using System.Windows.Forms;

namespace FileUtility
{
    public static class File
    {
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
