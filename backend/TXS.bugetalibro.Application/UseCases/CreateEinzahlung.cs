using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using TXS.bugetalibro.Application.Contracts;
using TXS.bugetalibro.Application.Contracts.Data;
using TXS.bugetalibro.Domain.Entities;
using TXS.bugetalibro.Domain.Logic;

namespace TXS.bugetalibro.Application.UseCases
{
    public static class CreateEinzahlung
    {
        public class Request : IRequest<decimal>
        {
            public decimal Betrag { get; set; }
            public DateTime? Datum { get; set; }
        }

        internal class Handler : IRequestHandler<Request, decimal>
        {
            private readonly IDataStore dataStore;
            private readonly IDateProvider dateProvider;

            public Handler(IDataStore dataStore, IDateProvider dateProvider)
            {
                this.dataStore = dataStore;
                this.dateProvider = dateProvider;
            }

            public async Task<decimal> Handle(Request request, CancellationToken cancellationToken)
            {
                var einzahlungen = this.dataStore.Set<Einzahlung>();
                var auszahlungen = this.dataStore.Set<Auszahlung>();
                var datum = request.Datum ?? this.dateProvider.Today;
                var einzahlung = new Einzahlung(datum, request.Betrag);
                einzahlungen.Insert(einzahlung);
                await this.dataStore.SaveChangesAsync(cancellationToken);

                return new BalanceQueryFacade(einzahlungen, auszahlungen).GetBalanceAt(datum.AddDays(+1));
            }
        }

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                this.RuleFor(x => x.Betrag).GreaterThan(0m);
                this.RuleFor(x => x.Betrag).ScalePrecision(2, 10);
            }
        }
    }
}
