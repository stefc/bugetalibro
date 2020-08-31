using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using TXS.bugetalibro.Application.Contracts.Data;
using TXS.bugetalibro.Application.UseCases;
using TXS.bugetalibro.Domain.Entities;
using TXS.bugetalibro.UnitTests.Helper;

namespace TXS.bugetalibro.UnitTests.Application
{
    public class CreateAuszahlungTest : ApplicationTest
    {
        public CreateAuszahlungTest(ApplicationFixture fixture) : base(fixture)
        {
        }

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

            // (A)ction
            var response = await this.Mediator.Send(request);

            // (A)ssert
            Assert.Equal(-5.99m, response);

            var dataStore = base.Get<IDataStore>();
            Assert.Equal(1, dataStore.Set<Auszahlung>().Count());
            Assert.Equal(1, dataStore.Set<Kategorie>().Count());
            var row = dataStore.Set<Auszahlung>().Single(record => record.Betrag == 5.99m);
            Assert.Equal(new DateTime(2019, 12, 24), row.Datum);
            Assert.Equal("Restaurantbesuche", row.Kategorie.Name);
            Assert.Equal("Schokobecher", row.Notiz);
        }

        [Fact]
        public async Task TestTwiceKategory()
        {
            // (A)range 
            var request = new CreateAuszahlung.Request()
            {
                Betrag = 5.99m,
                Kategorie = "Restaurantbesuche",
                Notiz = "Schokobecher"
            };

            // (A)ction
            await this.Mediator.Send(request);
            var response = await this.Mediator.Send(request);

            // (A)ssert
            Assert.Equal(-11.98m, response);

            var dataStore = base.Get<IDataStore>();
            Assert.Equal(2, dataStore.Set<Auszahlung>().Count());
            Assert.Equal(1, dataStore.Set<Kategorie>().Count());
            var rows = dataStore.Set<Auszahlung>().AsEnumerable();

            var firstRow = rows.First();
            var secondRow = rows.Last();

            Assert.NotEqual(firstRow.Id, secondRow.Id);
            Assert.Equal(firstRow.Kategorie.Id, secondRow.Kategorie.Id);
        }
    }
}
