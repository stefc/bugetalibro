using System;
using TXS.bugetalibro.Domain.Entities;
using Xunit;

namespace TXS.bugetalibro.UnitTests.Domain
{
    public class EinzahlungTest
    {
        [Theory]
        [InlineData(-100)]
        [InlineData(0)]
        public void TestInvalidBetrag(int betrag)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Einzahlung(DateTime.Today, betrag));
        }

        [Fact] 
        public void TestInvalidDatum()
        {
            Assert.Throws<ArgumentOutOfRangeException>((()=> new Einzahlung(new DateTime(2010,1,1,10,23,5), 100m)));
        }

        [Fact]
        public void TestCreateEinzahlung()
        {
            var datum = new DateTime(2010,1,1);
            var subject = new Einzahlung(datum, 100m);

            Assert.Equal(datum, subject.Datum);
            Assert.Equal(100m, subject.Betrag);
            Assert.NotEqual(Guid.Empty, subject.Id);
        }
    }
}
