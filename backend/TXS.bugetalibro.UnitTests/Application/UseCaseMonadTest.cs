using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FakeItEasy;
using TXS.bugetalibro.Application.Contracts;
using TXS.bugetalibro.Application.UseCases;
using TXS.bugetalibro.Domain.Entities;
using TXS.bugetalibro.Domain.Functors;
using static TXS.bugetalibro.Application.UseCases.CreateAuszahlung;

namespace TXS.bugetalibro.UnitTests.Application
{
    using Unit = System.ValueTuple;

    public class UseCaseMonadTest
    {

        private readonly IDateProvider dateProvider = A.Fake<IDateProvider>();

        [Fact]
        public async Task TestInitial()
        {
            // (A)range 
            var request = new CreateAuszahlung.Request()
            {
                Betrag = 5.99m,
                Kategorie = "Restaurantbesuche",
                Notiz = "Schokobecher"
            };

            A.CallTo(() => dateProvider.Today).Returns(new DateTime(2019, 12, 24));
            var program = CreateAuszahlung.Handler.CreateProgram(request);
            
            // (A)ct 
            var actual = await TestRunnerAsync.Run(program, new Env(dateProvider, null, CancellationToken.None));

            // (A)ssert
            Assert.Equal(new DateTime(2019,12,24), actual);
        }

        public static class TestRunnerAsync
        {
            public static async Task<A> Run<A>(IO<A> p, Env env)
            {
                switch (p)
                {
                    case Return<A> r:
                        return r.Result;

                    case IO<GetDatum, DateTime, A> x:
                        return await Run(x.As(i => x.Input.Datum ?? env.DateProvider.Today), env);

                    case IO<ReadKategorie, (bool, Kategorie, DateTime), A> x:
                        var kategorie = env.DataStore.Set<Kategorie>().SingleOrDefault(e => e.Name == x.Input.Name);
                        return await Run(x.As(i =>
                        {
                            return (kategorie == null, kategorie ?? new Kategorie(x.Input.Name), x.Input.Datum);
                        }), env);

                    case IO<WriteKategorie, Kategorie, A> x:
                        return await Run(x.As(i =>
                        {
                            if (x.Input.IsNew)
                            {
                                env.DataStore.Set<Kategorie>().Insert(x.Input.Kategorie);
                            }
                            return x.Input.Kategorie;
                        }), env);

                    case IO<WriteAuszahlung, DateTime, A> x:
                        return await Run(x.As(i =>
                        {
                            env.DataStore.Set<Auszahlung>().Insert(x.Input.Auszahlung);
                            return x.Input.Auszahlung.Datum;
                        }), env);

                    case IO<Commit, Unit, A> x:
                        return await Run(x.As(async i => await env.DataStore.SaveChangesAsync(env.CancellationToken)), env);

                    default: throw new NotSupportedException($"Not supported operation {p}");
                }
            }

        }
    }
}
