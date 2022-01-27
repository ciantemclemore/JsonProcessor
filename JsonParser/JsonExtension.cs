using System.Text;

namespace JParser
{
    public static class JsonExtension
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
                if (json[i] == '{' || json[i] == '[' || json[i] == ',')
                {
                    stringBuilder.Append(json[i]);
                    stringBuilder.Append("\n");

                    if (json[i] != ',') 
                    {
                        level++;
                    }

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
                    stringBuilder.Append(' ');
                }
                else 
                {
                    stringBuilder.Append(json[i]);
                }
            }

            return stringBuilder.ToString();
        }
    }
}
