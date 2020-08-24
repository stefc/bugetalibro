using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using TXS.bugetalibro.Application.Contracts;
using TXS.bugetalibro.Application.Contracts.Data;
using TXS.bugetalibro.Application.Models;
using TXS.bugetalibro.Domain.Facades;

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
            private readonly BalanceQueryFacade balanceQueryFacade;

            public Handler(IDateProvider dateProvider, IDataStore dataStore, BalanceQueryFacade balanceQueryFacade)
            {
                this.dateProvider = dateProvider;
                this.dataStore = dataStore;
                this.balanceQueryFacade = balanceQueryFacade;
            }
            public Task<ÜberblickModel> Handle(Request request, CancellationToken cancellationToken)
            {
                var datumStart = GetDatumStart(request);

                var dateRange = GetDatumRange(datumStart);

                var startSaldo = this.balanceQueryFacade.GetBalanceAt(dateRange.start);
                var endSaldo = this.balanceQueryFacade.GetBalanceAt(dateRange.end);
                var credits = this.balanceQueryFacade.GetCredits(dateRange);

                return Task.FromResult(ToViewModel((datumStart, startSaldo, endSaldo, credits)));
            }

            private DateTime GetDatumStart(Request request)
            {
                var hasDatum = request.Monat.HasValue && request.Jahr.HasValue;
                return hasDatum ?
                    new DateTime(request.Jahr.Value, request.Monat.Value, 1) :
                    this.dateProvider.Today.BeginOfMonth();
            }

            private (DateTime start, DateTime end) GetDatumRange(DateTime date)
            => (date.BeginOfMonth(), date.EndOfMonth());

            private ÜberblickModel ToViewModel(
                (DateTime datumStart, decimal startSaldo, decimal endSaldo, decimal credits) model)
            {
                return new ÜberblickModel()
                {
                    Monat = model.datumStart.Month,
                    Jahr = model.datumStart.Year,

                    StartSaldo = model.startSaldo,
                    EndSaldo = model.endSaldo,

                    SummeEinzahlungen = model.credits
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
    }
}
