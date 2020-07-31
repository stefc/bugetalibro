using System;
using Xunit;

using TXS.bugetalibro.Domain.ValueObjects;

namespace TXS.bugetalibro.UnitTests.Domain.ValueObjects
{
    public class KategorieTest
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void TestInvalid(string code)
        {
            Assert.Throws<ArgumentNullException>((() => new Kategorie(code)));
        }
    }
}
