using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FakeItEasy;
using TXS.bugetalibro.Application.Contracts;
using TXS.bugetalibro.Application.UseCases;
using TXS.bugetalibro.Domain.Entities;
using TXS.bugetalibro.Application.Contracts.Data;
using System.Collections.Generic;
using TXS.bugetalibro.UnitTests.Helper;
using TXS.bugetalibro.Domain.Functors;
using static TXS.bugetalibro.Application.UseCases.IOMonad;

namespace TXS.bugetalibro.UnitTests.Application
{
    public class UseCaseMonadTest
    {
        private readonly IDateProvider dateProvider = A.Fake<IDateProvider>();
        private readonly IDataStore dataStore = A.Fake<IDataStore>();

        // Hier wird mittels Mocks geprüft ob die Umgebung die korrekten Parameter erhält (keine Db nötig!)
        [Fact]
        public async Task TestInitialMockedEnvironment()
        {
            // (A)range 
            var request = new CreateAuszahlung.Request()
            {
                Betrag = 5.99m,
                Kategorie = "Restaurantbesuche",
                Notiz = "Schokobecher"
            };

            // Mock für IDateProvider
            A.CallTo(() => dateProvider.Today).Returns(new DateTime(2019, 12, 24));

            // Mock für IDataStore
            var fakedKategorien = TestOverrides.CreateDataSet(new List<Kategorie>());
            A.CallTo(() => dataStore.Set<Kategorie>()).Returns(fakedKategorien);
            var fakedAuszahlungen = TestOverrides.CreateDataSet(new List<Auszahlung>());
            A.CallTo(() => dataStore.Set<Auszahlung>()).Returns(fakedAuszahlungen);

            // (A)ct 
            var datum = await new Runner(new Env(dateProvider, dataStore, CancellationToken.None))
                    .Run(CreateAuszahlung.Handler.CreateProgram(request));

            // (A)ssert
            Assert.Equal(new DateTime(2019,12,25), datum);
            
            A.CallTo(() => fakedKategorien.Insert(A<Kategorie>.That.Matches(k=> k.Name.Equals(request.Kategorie))))
                .MustHaveHappened();
            A.CallTo(() => fakedAuszahlungen.Insert(
                A<Auszahlung>.That.Matches(a=> 
                    a.Betrag == 5.99m 
                    && a.Datum == new DateTime(2019,12,24)
                    && a.Notiz == request.Notiz 
                    && a.Kategorie.Name == request.Kategorie)))
                .MustHaveHappened();

            A.CallTo(() => dataStore.SaveChangesAsync(A<CancellationToken>._)).MustHaveHappened();
        }

        // Hier wird nur das Durchreichen des korrekten Datums durch die Pipeline geprüft
        [Fact]
        public void TestSyncRunner()
        {
            // (A)range 
            var request = new CreateAuszahlung.Request()
            {
                Betrag = 5.99m,
                Kategorie = "Restaurantbesuche",
                Notiz = "Schokobecher"
            };

            // (A)ct 
            var runner = new TestRunner();
            var datum = runner.Run(CreateAuszahlung.Handler.CreateProgram(request));

            // (A)ssert
            Assert.Equal(new DateTime(2019,12,25), datum);
        }

        public class TestRunner
        {
            // Example of non-recursive (stack-safe) interpreter
            public A Run<A>(IO<A> p)
            {
                while (true)
                    switch (p)
                    {
                        case Return<A> x:
                            return x.Result;
                        
                        case IO<GetDatum, DateTime, A> x:
                           p = x.Next(new DateTime(2019,12,24));
                           break;

                        case IO<ReadKategorie, (bool, Kategorie, DateTime), A> x:
                            p = x.Next((true,new Kategorie(x.Input.Name), x.Input.Datum));
                            break;

                        case IO<WriteKategorie, (Kategorie,DateTime), A> x:
                            p = x.Next((x.Input.Kategorie, x.Input.Datum));
                            break;

                        case IO<WriteAuszahlung, DateTime, A> x:
                            p = x.Next(x.Input.Auszahlung.Datum);
                            break;

                        case IO<Commit, DateTime, A> x:
                            p = x.Next(x.Input.Datum);
                            break;
                            
                        default: throw new NotSupportedException($"Not supported operation {p}");
                    }
            }
        }
    }
}
