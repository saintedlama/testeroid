using System;
using Fixture.Core;

namespace Fixture
{
    public static class ExtendedMathOperations
    {
        public static double Add(params double?[] operands)
        {
            double? result = 0;

            foreach (var operand in operands)
            {
                result = MathOperations.Add(result, operand);
            }

            return result.Value;
        }
    }
}
