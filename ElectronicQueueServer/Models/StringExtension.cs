using System;

namespace ElectronicQueueServer.Models
{
    public static class StringExtension
    {
        public static string Capitalize(this string source)
        {
            switch (source)
            {
                case null: throw new ArgumentNullException(nameof(source));
                case "": throw new ArgumentException($"{nameof(source)} cannot be empty", nameof(source));
                default: return source[0].ToString().ToUpper() + source.Substring(1);
            }
        }
    }
}
