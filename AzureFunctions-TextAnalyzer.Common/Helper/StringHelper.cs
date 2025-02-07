using System.Text.RegularExpressions;

namespace AzureFunctions_TextAnalyzer.Common.Helper
{
    public class StringHelper
    {
        public static int FindIndexOfLastSpace(string input)
        {
            int lastIndex = input.LastIndexOf(' ');

            return lastIndex >= 0 ? lastIndex : 0;
        }

        public static string ReplaceNonWordWithSpace(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            string result = Regex.Replace(text, @"[\W_]+", " ");

            result = result.Trim();

            return result;
        }

        public static string GeneratePartitionKey(string fileName, int chunkCount)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name cannot be null or whitespace.", nameof(fileName));

            return $"{fileName.Replace(" ", "_")}_Chunks_{chunkCount}";

        }

    }
}
