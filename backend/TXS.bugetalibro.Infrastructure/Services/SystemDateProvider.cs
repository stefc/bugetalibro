using System;
using TXS.bugetalibro.Application.Contracts;

namespace TXS.bugetalibro.Infrastructure.Services
{
    internal class SystemDateProvider : IDateProvider
    {
        public DateTime Today => DateTime.Today;
    }
}
