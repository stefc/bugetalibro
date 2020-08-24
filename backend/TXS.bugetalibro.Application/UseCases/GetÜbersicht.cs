using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using NodaTime;
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
                var hasDatum = request.Monat.HasValue && request.Jahr.HasValue;

                var datumStart = hasDatum ?
                    new LocalDate(request.Jahr.Value, request.Monat.Value, 1)
                    :
                    new LocalDate(this.dateProvider.Today.Year, this.dateProvider.Today.Month, 1);

                (DateTime start, DateTime end) dateRange = (datumStart.ToDateTimeUnspecified(),
                    datumStart.PlusMonths(1).PlusDays(-1).ToDateTimeUnspecified());

                var startSaldo = this.balanceQueryFacade.GetBalanceAt(dateRange.start);
                var endSaldo = this.balanceQueryFacade.GetBalanceAt(dateRange.end);

                var credits = this.balanceQueryFacade.GetCredits(dateRange);

                return Task.FromResult(ToViewModel((datumStart, startSaldo, endSaldo, credits)));
            }

            private ÜberblickModel ToViewModel(
                (LocalDate datumStart, decimal startSaldo, decimal endSaldo, decimal credits) model)
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
}
