namespace EasyNow.Utility.Extensions
{
    public static class CharExtensions
    {
        public static char ToLower(this char c)
        {
#if HAVE_CHAR_TO_LOWER_WITH_CULTURE
            c = char.ToLower(c, CultureInfo.InvariantCulture);
#else
            c = char.ToLowerInvariant(c);
#endif
            return c;
        }
    }
}