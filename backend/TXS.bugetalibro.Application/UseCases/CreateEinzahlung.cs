using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using TXS.bugetalibro.Application.Contracts;
using TXS.bugetalibro.Application.Contracts.Data;
using TXS.bugetalibro.Domain.Entities;

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
                var einzahlung = new Einzahlung(request.Datum ?? this.dateProvider.Today, request.Betrag);
                this.dataStore.Set<Einzahlung>().Insert(einzahlung);
                await this.dataStore.SaveChangesAsync(cancellationToken);

                var sumEinzahlungen = this.dataStore.Set<Einzahlung>().Sum(e => e.Betrag);
                var sumAuszahlungen = this.dataStore.Set<Auszahlung>().Sum(e => e.Betrag);

                return sumEinzahlungen - sumAuszahlungen;
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
