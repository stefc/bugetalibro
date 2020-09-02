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
                var kategorien = this.dataStore.Set<Kategorie>();
                var datum = request.Datum ?? this.dateProvider.Today;
                var kategory = kategorien.SingleOrDefault( e => e.Name == request.Kategorie) 
                    ?? CreateNewKategorie(request);
                var auszahlung = new Auszahlung(datum, request.Betrag, kategory, request.Notiz);
                auszahlungen.Insert(auszahlung);
                await this.dataStore.SaveChangesAsync(cancellationToken);

                return new BalanceQueryFacade(einzahlungen, auszahlungen).GetBalanceAt(datum.AddDays(+1));
            }

            private Kategorie CreateNewKategorie(Request request) {
                var kategory = new Kategorie(request.Kategorie);
                this.dataStore.Set<Kategorie>().Insert(kategory);
                return kategory;
            }

           public static IO<Unit> CreateProgram(Request request) =>
             GetDatum(request.Datum)
                .Bind( _ => ReadKategorie(request.Kategorie, _))
                .Bind( _ => WriteKategorie(_))
                .Bind( _ => Log("Hello World"))
                .Bind( _ => Commit()); 

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

            public ReadKategorie(string name, DateTime datum) => (Name,Datum) = (name, datum);
        }

        public readonly struct WriteKategorie 
        {
            public readonly bool IsNew; 
            public readonly Kategorie Kategorie; 

            public readonly DateTime Datum;

            public WriteKategorie((bool isNew,Kategorie kategorie,DateTime datum) tuple) 
            => (IsNew,Kategorie,Datum) = (tuple.isNew,tuple.kategorie,tuple.datum);
        }

        public readonly struct Log
        {
            public readonly string Message;
            public Log(string message) => Message = message;
        }

        public readonly struct Commit
        {            
        }


        public static class ProgramOperations
        {
            public static IO<DateTime> GetDatum(DateTime? datum) =>
                new GetDatum(datum).ToIO<GetDatum, DateTime>();
            public static IO<(bool,Kategorie,DateTime)> ReadKategorie(string name,DateTime datum) =>
                new ReadKategorie(name,datum).ToIO<ReadKategorie, (bool,Kategorie,DateTime)>();

            public static IO<(Kategorie,DateTime)> WriteKategorie((bool,Kategorie,DateTime) kategorie) =>
                new WriteKategorie(kategorie).ToIO<WriteKategorie,(Kategorie,DateTime)>();

            public static IO<Unit> Log(string message) =>
                new Log(message).ToIO();

            public static IO<Unit> Commit() => 
                new Commit().ToIO();
 
        }


        public readonly struct Env 
        {
            public readonly IDateProvider DateProvider; 
            public readonly IDataStore DataStore; 
            public readonly CancellationToken CancellationToken;

            public Env(IDateProvider dateProvider, IDataStore dataStore, CancellationToken cancellationToken) 
            => (DateProvider,DataStore,CancellationToken) = (dateProvider,dataStore,cancellationToken);
        }
        public static class LiveRunnerAsync
        {
            public static async Task<A> Run<A>(IO<A> p, Env env)
            {
                switch (p)
                {
                    case Return<A> r:
                        return r.Result;

                    case IO<GetDatum, DateTime, A> x:
                        return await Run(x.As(i => x.Input.Datum ?? env.DateProvider.Today), env);

                    case IO<ReadKategorie, (bool,Kategorie,DateTime), A> x:
                        var kategorie = env.DataStore.Set<Kategorie>().SingleOrDefault(e => e.Name == x.Input.Name);
                        return await Run(x.As(i => {
                            return (kategorie == null, kategorie ?? new Kategorie(x.Input.Name), x.Input.Datum);
                        }), env);

                    case IO<WriteKategorie, Kategorie, A> x:
                        return await Run(x.As(i => {
                            if (x.Input.IsNew) 
                            {
                                env.DataStore.Set<Kategorie>().Insert(x.Input.Kategorie);
                            }
                            return x.Input.Kategorie;
                        }), env);

                    case IO<Log, Unit, A> x:
                        return await Run(x.As(i => Console.WriteLine(i.Message)),env); 

                    case IO<Commit, Unit, A> x:
                        return await Run(x.As(async i => await env.DataStore.SaveChangesAsync(env.CancellationToken)), env);

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
