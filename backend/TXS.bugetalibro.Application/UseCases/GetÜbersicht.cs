using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using NodaTime;
using TXS.bugetalibro.Application.Contracts;
using TXS.bugetalibro.Application.Contracts.Data;
using TXS.bugetalibro.Application.Models;
using TXS.bugetalibro.Domain.Entities;

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
                var hasDatum = request.Monat.HasValue && request.Jahr.HasValue;

                var datumStart = hasDatum ?
                    new LocalDate(request.Jahr.Value, request.Monat.Value, 1)
                    :
                    new LocalDate(this.dateProvider.Today.Year, this.dateProvider.Today.Month, 1);

                (DateTime start, DateTime end) dateRange = (datumStart.ToDateTimeUnspecified(),
                    datumStart.PlusMonths(1).PlusDays(-1).ToDateTimeUnspecified());

                var startSaldo = this.dataStore.Set<Einzahlung>()
                    .Where(e => e.Datum < dateRange.start).Sum(e => e.Betrag);

                var endSaldo = this.dataStore.Set<Einzahlung>()
                    .Where(e => e.Datum <= dateRange.end && e.Datum >= dateRange.start).Sum(e => e.Betrag);

                var überblickModel = new ÜberblickModel()
                {
                    Monat = datumStart.Month,
                    Jahr = datumStart.Year,

                    StartSaldo = startSaldo,
                    EndSaldo = startSaldo + endSaldo
                };
                return Task.FromResult(überblickModel);
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
