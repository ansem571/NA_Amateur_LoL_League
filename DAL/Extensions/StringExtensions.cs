using System.Text;

namespace DAL.Extensions
{
    internal static class StringExtensions
    {
        public static string TrimEnd(this StringBuilder sb)
        {
            return sb.ToString().TrimEnd(',', ' ', '|');
        }

        public static string Bracket(this string item, char startBracket, char endBracket)
        {
            return $"{startBracket}{item}{endBracket}";
        }

        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }
    }
}
