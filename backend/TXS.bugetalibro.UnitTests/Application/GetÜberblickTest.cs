using System.Threading.Tasks;
using TXS.bugetalibro.Application;
using TXS.bugetalibro.Application.UseCases;
using TXS.bugetalibro.UnitTests.Helper;
using Xunit;

namespace TXS.bugetalibro.UnitTests.Application
{
    public class GetÜberblickTest : ApplicationTest
    {
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
    }
}
