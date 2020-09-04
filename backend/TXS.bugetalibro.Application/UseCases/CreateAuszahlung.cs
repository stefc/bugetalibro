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

    using static F;

    using Unit = System.ValueTuple;

    using static CreateAuszahlung.ProgramOperations;

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
                var einzahlungen = this.dataStore.Set<Einzahlung>();
                var auszahlungen = this.dataStore.Set<Auszahlung>();
                var datum = request.Datum ?? this.dateProvider.Today;
 
                await new Runner(new Env(this.dateProvider, this.dataStore, cancellationToken))
                    .Run(CreateProgram(request));

                return new BalanceQueryFacade(einzahlungen, auszahlungen).GetBalanceAt(datum.AddDays(+1));
            }

            public static IO<Unit> CreateProgram(Request request) =>
                GetDatum(request.Datum)
                    .Bind(_ => ReadKategorie(request.Kategorie, _))
                    .Bind(_ => WriteKategorie(_))
                    .Bind(_ => WriteAuszahlung(new Auszahlung(_.datum, request.Betrag, _.kategorie, request.Notiz)))
                    .Bind(_ => Commit(_)); 
        }


        // Program description

        public readonly struct GetDatum
        {
            public readonly DateTime? Datum;
            public GetDatum(DateTime? datum) => (Datum) = (datum);
        }

        public readonly struct ReadKategorie
        {
            public readonly string Name;
            public readonly DateTime Datum;
            public ReadKategorie(string name, DateTime datum) => (Name, Datum) = (name, datum);
        }

        public readonly struct WriteKategorie
        {
            public readonly bool IsNew;
            public readonly Kategorie Kategorie;

            public readonly DateTime Datum;

            public WriteKategorie((bool isNew, Kategorie kategorie, DateTime datum) tuple)
            => (IsNew, Kategorie, Datum) = (tuple.isNew, tuple.kategorie, tuple.datum);
        }

        public readonly struct WriteAuszahlung
        {
            public readonly Auszahlung Auszahlung;

            public WriteAuszahlung(Auszahlung auszahlung)
            => (Auszahlung) = (auszahlung);
        }

        public readonly struct Commit
        {
            public readonly DateTime Datum;

            public Commit(DateTime datum) => (Datum) = (datum);
        }


        public static class ProgramOperations
        {
            public static IO<DateTime> GetDatum(DateTime? datum) =>
                new GetDatum(datum).ToIO<GetDatum, DateTime>();
            public static IO<(bool, Kategorie, DateTime)> ReadKategorie(string name, DateTime datum) =>
                new ReadKategorie(name, datum).ToIO<ReadKategorie, (bool, Kategorie, DateTime)>();

            public static IO<(Kategorie kategorie, DateTime datum)> WriteKategorie((bool, Kategorie, DateTime) kategorie) =>
                new WriteKategorie(kategorie).ToIO<WriteKategorie, (Kategorie kategorie, DateTime datum)>();


            public static IO<DateTime> WriteAuszahlung(Auszahlung auszahlung) =>
                new WriteAuszahlung(auszahlung).ToIO<WriteAuszahlung, DateTime>();

            public static IO<Unit> Commit(DateTime datum) =>
                new Commit(datum).ToIO<Commit, Unit>();
        }


        public readonly struct Env
        {
            public readonly IDateProvider DateProvider;
            public readonly IDataStore DataStore;
            public readonly CancellationToken CancellationToken;

            public Env(IDateProvider dateProvider, IDataStore dataStore, CancellationToken cancellationToken)
            => (DateProvider, DataStore, CancellationToken) = (dateProvider, dataStore, cancellationToken);
        }
        
        public class Runner
        {
            private readonly Env env; 

            public Runner(Env env)
            {
                this.env=env;
            }
            public async Task<A> Run<A>(IO<A> p)
            {
                switch (p)
                {
                    case Return<A> r:
                        return r.Result;

                    case IO<GetDatum, DateTime, A> x:
                        return await Run(x.As(i => x.Input.Datum ?? env.DateProvider.Today));

                    case IO<ReadKategorie, (bool, Kategorie, DateTime), A> x:
                        var kategorie = env.DataStore.Set<Kategorie>().SingleOrDefault(e => e.Name == x.Input.Name);
                        return await Run(x.As(i =>
                        {
                            return (kategorie == null, kategorie ?? new Kategorie(x.Input.Name), x.Input.Datum);
                        }));

                    case IO<WriteKategorie, (Kategorie,DateTime), A> x:
                        return await Run(x.As(i =>
                        {
                            if (x.Input.IsNew)
                            {
                                env.DataStore.Set<Kategorie>().Insert(x.Input.Kategorie);
                            }
                            return (x.Input.Kategorie,x.Input.Datum);
                        }));

                    case IO<WriteAuszahlung, DateTime, A> x:
                        return await Run(x.As(i =>
                        {
                            env.DataStore.Set<Auszahlung>().Insert(x.Input.Auszahlung);
                            return x.Input.Auszahlung.Datum;
                        }));

                    case IO<Commit, Unit, A> x:
                        return await Run(x.As(async i => await env.DataStore.SaveChangesAsync(env.CancellationToken)));
                    
                    default: throw new NotSupportedException($"Not supported operation {p}");
                }
            }

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
