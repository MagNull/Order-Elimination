using System;

namespace OrderElimination.Infrastructure
{
    public static class MathStringExtensions
    {
        public static string AsString(this MathOperation operation)
        {
            return operation switch
            {
                MathOperation.Add => "+",
                MathOperation.Subtract => "-",
                MathOperation.Multiply => "*",
                MathOperation.Divide => "/",
                _ => throw new NotImplementedException(),
            };
        }
    }
}
