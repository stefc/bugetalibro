
using System;

namespace TXS.bugetalibro.Domain
{
    public static class DateTimeExtensions
    {
        public static DateTime BeginOfMonth(this DateTime date)
        => new DateTime(date.Year, date.Month, 1);

        public static DateTime EndOfMonth(this DateTime date)
        => new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));

        public static (DateTime start, DateTime end) GetMonthRange(this DateTime date)
        => (date.BeginOfMonth(), date.EndOfMonth());
    }
}
