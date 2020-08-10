using System;
using TXS.bugetalibro.Domain.ValueObjects;
using Xunit;

namespace TXS.bugetalibro.UnitTests.Domain
{
    public class KategorieTest
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void TestInvalid(string code)
        {
            Assert.Throws<ArgumentNullException>(() => new Kategorie(code));
        }
    }
}
