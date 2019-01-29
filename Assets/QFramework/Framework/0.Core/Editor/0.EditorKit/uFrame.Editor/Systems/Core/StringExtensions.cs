using System.Text.RegularExpressions;

namespace QFramework.GraphDesigner
{
    public static class StringExtensions
    {
        public static string PrettyLabel(this string label)
        {
            return Regex.Replace(label, @"[^\w\s]|_", "");
        }
    }
}