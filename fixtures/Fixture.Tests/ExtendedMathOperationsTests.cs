using System;
using Xunit;
using Fixture;

namespace Fixture.Tests
{
    public class ExtendedMathOperationsTests
    {
        [Fact]
        public void ShouldAddOperands()
        {
            var result = ExtendedMathOperations.Add(1,2,3);

            Assert.Equal(6, result);
        }
    }
}
