namespace Classy.DotNet.Mvc.Extensions
{
    public static class StringExtensions
    {
        public static string With(this string str, params object[] args)
        {
            return string.Format(str, args);
        }
    }
}