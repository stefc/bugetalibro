using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using TXS.bugetalibro.Application.Contracts;
using TXS.bugetalibro.Application.Contracts.Data;
using TXS.bugetalibro.Domain.Entities;
using TXS.bugetalibro.Domain.Functors;
using TXS.bugetalibro.Domain.Logic;

namespace TXS.bugetalibro.Application.UseCases
{

    using static IOMonad.Operations;
    using static TXS.bugetalibro.Application.UseCases.IOMonad;

    public static class CreateAuszahlung
    {
        public class Request : IRequest<decimal>
        {
            public decimal Betrag { get; set; }
            public DateTime? Datum { get; set; }
            public string Kategorie { get; set; }
            public string Notiz { get; set; }
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
                var datum = await new Runner(new Env(this.dateProvider, this.dataStore, cancellationToken))
                    .Run(CreateProgram(request));

                var einzahlungen = this.dataStore.Set<Einzahlung>();
                var auszahlungen = this.dataStore.Set<Auszahlung>();
                return new BalanceQueryFacade(einzahlungen, auszahlungen).GetBalanceAt(datum);
            }

            public static IO<DateTime> CreateProgram(Request request) =>
                GetDatum(request.Datum)
                    .SelectMany(_ => ReadKategorie(request.Kategorie, _))
                    .SelectMany(_ => WriteKategorie(_))
                    .SelectMany(_ => WriteAuszahlung(new Auszahlung(_.datum, request.Betrag, _.kategorie, request.Notiz)))
                    .SelectMany(_ => Commit(_.AddDays(+1))); 
        }

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                this.RuleFor(x => x.Betrag).GreaterThan(0m);
                this.RuleFor(x => x.Betrag).ScalePrecision(2, 10);
                this.RuleFor(x => x.Kategorie).NotEmpty();
            }
        }
    }
}
