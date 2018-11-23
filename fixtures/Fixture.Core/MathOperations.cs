using System;

namespace Fixture.Core
{
    public static class MathOperations
    {
        public static double Add(double? left, double? right)
        {
            if (!left.HasValue)
            {
                return Add(0, right);
            }

            if (!right.HasValue)
            {
                return Add(left, 0);
            }

            return left.Value + right.Value;
        }
    }
}
