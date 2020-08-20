using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

    public static class OverridesExtensions {

        public static IServiceCollection AddTestDateProvider(this IServiceCollection services)
            => services.Replace(ServiceDescriptor.Transient<IDateProvider>(sp => new TestOverrides.DateProvider()));
    }
}
