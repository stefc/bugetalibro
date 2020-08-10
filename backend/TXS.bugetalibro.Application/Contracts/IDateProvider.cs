using System;

namespace TXS.bugetalibro.Application.Contracts
{
    public interface IDateProvider
    {
        DateTime Today { get; }
    }
}
