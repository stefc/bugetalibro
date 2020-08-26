using System;
using TXS.bugetalibro.Domain.Entities;
using Xunit;

namespace TXS.bugetalibro.UnitTests.Domain
{
    public class KategorieTest
    {
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
