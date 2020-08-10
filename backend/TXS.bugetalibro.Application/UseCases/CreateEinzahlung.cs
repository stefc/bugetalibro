using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using TXS.bugetalibro.Application.Contracts.Data;
using TXS.bugetalibro.Domain.Entities;

namespace TXS.bugetalibro.Application.UseCases
{
    public static class CreateEinzahlung
    {
        public class Request : IRequest<decimal>
        {
            public decimal Betrag { get; set; }
            public DateTime Datum { get; set; }
        }

        internal class Handler : IRequestHandler<Request, decimal>
        {
            private readonly IDataStore dataStore;

            public Handler(IDataStore dataStore)
            {
                this.dataStore = dataStore;
            }
            
            public async Task<decimal> Handle(Request request, CancellationToken cancellationToken)
            {
                var einzahlung = new Einzahlung(request.Datum, request.Betrag);
                this.dataStore.Set<Einzahlung>().Insert(einzahlung);
                await this.dataStore.SaveChangesAsync(cancellationToken);
                
                var kassenbestand = 0m; // TODO: Kassenbestand ermitteln
                return kassenbestand;
            }
        }

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                this.RuleFor(req => req.Betrag).GreaterThan(0m);
                this.RuleFor(req => req.Datum).NotEmpty();
            }
        }
    }
}
