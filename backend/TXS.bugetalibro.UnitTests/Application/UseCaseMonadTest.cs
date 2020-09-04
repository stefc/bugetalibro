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
using TXS.bugetalibro.Application.Contracts.Data;
using System.Collections.Generic;
using TXS.bugetalibro.UnitTests.Helper;

namespace TXS.bugetalibro.UnitTests.Application
{
    using Unit = System.ValueTuple;

    public class UseCaseMonadTest
    {

        private readonly IDateProvider dateProvider = A.Fake<IDateProvider>();
        private readonly IDataStore dataStore = A.Fake<IDataStore>();

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

            // Mock für IDateProvider
            A.CallTo(() => dateProvider.Today).Returns(new DateTime(2019, 12, 24));

            // Mock für IDataStore
            var fakedKategorien = TestOverrides.CreateDataSet(new List<Kategorie>());
            A.CallTo(() => dataStore.Set<Kategorie>()).Returns(fakedKategorien);
            var fakedAuszahlungen = TestOverrides.CreateDataSet(new List<Auszahlung>());
            A.CallTo(() => dataStore.Set<Auszahlung>()).Returns(fakedAuszahlungen);

            // (A)ct 
            await new CreateAuszahlung.Runner(new Env(dateProvider, dataStore, CancellationToken.None))
                    .Run(CreateAuszahlung.Handler.CreateProgram(request));

            // (A)ssert
            
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

        private IDataSet<T> CreateFakeIDataSet<T>(IList<T> container) where T : class
        {
            IQueryable<T> fakeIQueryable = new List<T>().AsQueryable();
            var fakeDbSet = A.Fake<IDataSet<T>>((d => d.Implements(typeof(IQueryable<T>))));

            A.CallTo(() => ((IQueryable<T>)fakeDbSet).GetEnumerator())
                .Returns(fakeIQueryable .GetEnumerator());

            A.CallTo(() => ((IQueryable<T>)fakeDbSet).Provider)
                .Returns(fakeIQueryable .Provider);

            A.CallTo(() => ((IQueryable<T>)fakeDbSet).Expression)
                .Returns(fakeIQueryable .Expression);

            A.CallTo(() => ((IQueryable<T>)fakeDbSet).ElementType)
                .Returns(fakeIQueryable .ElementType);

            return fakeDbSet;
        }

        
    }
}
