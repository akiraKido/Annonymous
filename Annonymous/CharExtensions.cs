namespace Annonymous
{
    internal static class CharExtensions
    {
        internal static bool IsWhiteSpace(this char c) => char.IsWhiteSpace(c);
        internal static bool IsLetter(this char c) => char.IsLetter(c);
        internal static bool IsLetterOrDigit(this char c) => char.IsLetterOrDigit(c);
        internal static bool IsDigit(this char c) => char.IsLetterOrDigit(c);
    }
}