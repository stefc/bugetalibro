using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TXS.bugetalibro.Application;
using TXS.bugetalibro.Application.Contracts.Data;
using TXS.bugetalibro.Application.UseCases;
using TXS.bugetalibro.Domain.Entities;
using TXS.bugetalibro.Infrastructure.Persistence;
using TXS.bugetalibro.UnitTests.Helper;
using Xunit;

namespace TXS.bugetalibro.UnitTests.Application
{
    public class GetÜberblickTest : ApplicationTest
    {
        public GetÜberblickTest(ApplicationFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task TestInitial()
        {
            // (A)range 
            var request = new GetÜbersicht.Request();

            // (A)ction
            var response = await this.Mediator.Send(request);
            
            // (A)ssert
            Assert.NotNull(response);
            Assert.Equal(0m, response.StartSaldo);
            Assert.Equal(0m, response.EndSaldo);
            Assert.Equal(12, response.Monat);
            Assert.Equal(2019, response.Jahr);
        }

        [Fact]
        public async Task TestDediziertesDatum() {
            // (A)range 
            var request = new GetÜbersicht.Request() { Monat = 2, Jahr = 2020 };

            // (A)ction
            var response = await this.Mediator.Send(request);

            // (A)ssert
            Assert.NotNull(response);
            Assert.Equal(0m, response.StartSaldo);
            Assert.Equal(0m, response.EndSaldo);
            Assert.Equal(2, response.Monat);
            Assert.Equal(2020, response.Jahr);
        }

        [Fact]
        public async Task TestFullDb() 
        {
            // Arrange
            await base.UseSampleDb(TestOverrides.SampleDb);

            // Assert 
            var dataStore = base.Get<IDataStore>();
            Assert.Equal(3, dataStore.Set<Einzahlung>().Count());
            // Assert.Equal(343.00m, dataStore.Set<Einzahlung>().Sum( x => x.Betrag));
        }
    }
}
