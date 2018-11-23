using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fixture.Core.Tests
{
    [TestClass]
    public class MathOperationsTests
    {
        [TestMethod]
        public void ShouldAddOperands()
        {
            var result = MathOperations.Add(1,2);

            Assert.AreEqual(3, result);
        }
    }
}
