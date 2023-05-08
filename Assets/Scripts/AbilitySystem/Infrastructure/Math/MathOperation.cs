using System;
using System.Security;

namespace OrderElimination.AbilitySystem
{
    public enum MathOperation
    {
        Add,
        Subtract,
        Multiply,
        Divide,
    }

    public static class MathOperationExtensions
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
