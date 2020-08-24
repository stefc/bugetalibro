using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
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
    public class GetÜbersichtTest : ApplicationTest
    {
        public GetÜbersichtTest(ApplicationFixture fixture) : base(fixture)
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

        [Theory]
        [InlineData(11,1968, 0, 0)]
        [InlineData(12,1968, 0, 100.01)]
        [InlineData( 1,1969, 100.01, 100.01)]
        [InlineData( 2,2006, 301.00, 343.00)]
        public async Task TestSaldoAt(int month, int year, decimal expectedStart, decimal expectedEnd)
        {
            // Arrange
            await base.UseSampleDb(TestOverrides.SampleDb);
            var request = new GetÜbersicht.Request() { Monat = month, Jahr = year };

            // (A)ction
            var response = await this.Mediator.Send(request);

            // Assert 
            Assert.Equal(expectedStart, response.StartSaldo);
            Assert.Equal(expectedEnd, response.EndSaldo);
        }

        [Theory]
        [InlineData(1, 2020, null)]
        [InlineData(1, null, 4)]
        [InlineData(1, 2020, 0)]
        [InlineData(1, 2020, 13)]
        public async Task TestValidate(int expected, int? year, int? month)
        {
            // (A)range 
            var request = new GetÜbersicht.Request() { Jahr = year, Monat = month };

            // (A)ction
            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => this.Mediator.Send(request));

            // (A)ssert
            Assert.Equal(expected, exception.Errors.Count());
        }
    }
}
