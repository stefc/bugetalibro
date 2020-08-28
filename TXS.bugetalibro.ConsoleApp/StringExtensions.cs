
using System;

namespace TXS.bugetalibro.ConsoleApp
{
    public static class StringExtensions
    {
        public static string Align(this string value, int width)
        => Math.Sign(width) == +1 ? value.PadRight(width) : value.PadLeft(Math.Abs(width));
    }
}
