using System;
using Xunit;

using TXS.buĝetalibro.Domain.ValueObjects;

namespace TXS.buĝetalibro.UnitTests.Domain.ValueObjects
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
