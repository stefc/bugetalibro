using System;
using TXS.bugetalibro.Domain.Entities;
using Xunit;

namespace TXS.bugetalibro.UnitTests.Domain
{
    public class KategorieTest
    {

        [Fact]
        [Trait("Category","Unit")]
        public void TestCreateKategorie()
        {
            var subject = new Kategorie("Miete");

            Assert.Equal("Miete", subject.Name);
            Assert.NotEqual(Guid.Empty, subject.Id);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [Trait("Category","Unit")]
        public void TestInvalid(string code)
        {
            Assert.Throws<ArgumentNullException>(() => new Kategorie(code));
        }
    }
}
