using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using TXS.bugetalibro.Application.Contracts;
using TXS.bugetalibro.Application.Contracts.Data;
using TXS.bugetalibro.Domain.Entities;
using TXS.bugetalibro.Domain.Facades;

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

            private readonly BalanceQueryFacade balanceQueryFacade;

            public Handler(IDataStore dataStore, IDateProvider dateProvider, BalanceQueryFacade balanceQueryFacade)
            {
                this.dataStore = dataStore;
                this.dateProvider = dateProvider;
                this.balanceQueryFacade = balanceQueryFacade;
            }
            
            public async Task<decimal> Handle(Request request, CancellationToken cancellationToken)
            {
                var datum = request.Datum ?? this.dateProvider.Today;
                var einzahlung = new Einzahlung(datum, request.Betrag);
                this.dataStore.Set<Einzahlung>().Insert(einzahlung);
                await this.dataStore.SaveChangesAsync(cancellationToken);
                return this.balanceQueryFacade.GetBalanceAt(datum.AddDays(+1));
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
