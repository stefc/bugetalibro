using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using TXS.bugetalibro.Application.Contracts;
using TXS.bugetalibro.Application.Contracts.Data;
using TXS.bugetalibro.Application.Models;
using TXS.bugetalibro.Domain.Entities;
using TXS.bugetalibro.Domain.Logic;

namespace TXS.bugetalibro.Application.UseCases
{
    public static class GetÜbersicht
    {
        public class Request : IRequest<ÜberblickModel>
        {
            public int? Monat { get; set; }
            public int? Jahr { get; set; }
        }

        internal class Handler : IRequestHandler<Request, ÜberblickModel>
        {
            private readonly IDataStore dataStore;
            private readonly IDateProvider dateProvider;

            public Handler(IDateProvider dateProvider, IDataStore dataStore)
            {
                this.dateProvider = dateProvider;
                this.dataStore = dataStore;
            }
            public Task<ÜberblickModel> Handle(Request request, CancellationToken cancellationToken)
            {
                var dateRange =GetDatumStart(request).GetMonthRange();

                var einzahlungen = this.dataStore.Set<Einzahlung>();
                var auszahlungen = this.dataStore.Set<Auszahlung>();
                var balanceQueryFacade = new BalanceQueryFacade(einzahlungen, auszahlungen);

                return Task.FromResult(ToViewModel(dateRange, balanceQueryFacade));
            }

            private DateTime GetDatumStart(Request request)
            {
                var hasDatum = request.Monat.HasValue && request.Jahr.HasValue;
                return hasDatum ?
                    new DateTime(request.Jahr.Value, request.Monat.Value, 1) :
                    this.dateProvider.Today.BeginOfMonth();
            }

            
            private ÜberblickModel ToViewModel(
                (DateTime start, DateTime end) dateRange, BalanceQueryFacade balanceQueryFacade)
            {
                return new ÜberblickModel()
                {
                    Monat = dateRange.start.Month,
                    Jahr = dateRange.start.Year,

                    StartSaldo = balanceQueryFacade.GetBalanceAt(dateRange.start),
                    EndSaldo = balanceQueryFacade.GetBalanceAt(dateRange.end),

                    SummeEinzahlungen = balanceQueryFacade.GetCredits(dateRange)
                };
            }
        }

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                this.RuleFor(req => req.Monat).NotNull().When(req => req.Jahr.HasValue).WithMessage("Monat fehlt");
                this.RuleFor(req => req.Jahr).NotNull().When(req => req.Monat.HasValue).WithMessage("Jahr fehlt");
                this.RuleFor(req => req.Monat).InclusiveBetween(1, 12);
            }
        }
    }

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
