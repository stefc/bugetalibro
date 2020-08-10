using System;
using TXS.bugetalibro.Application.Contracts;

namespace TXS.bugetalibro.UnitTests.Helper
{
    internal static class TestOverrides
    {
        public class DateProvider : IDateProvider
        {
            public DateTime Today { get; }

            public DateProvider(DateTime? date = null)
            {
                this.Today = date ?? new DateTime(2019, 12, 24);
            }
        }
    }
}
