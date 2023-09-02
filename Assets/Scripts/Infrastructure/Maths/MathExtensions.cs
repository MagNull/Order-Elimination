using System;
using UnityEngine;

namespace OrderElimination.Infrastructure
{
    public static class MathExtensions
    {
        public static int Round(float value, RoundingOption mode)
        {
            return mode switch
            {
                RoundingOption.Math => (int)Math.Round(value, MidpointRounding.AwayFromZero),
                RoundingOption.Floor => Mathf.FloorToInt(value),
                RoundingOption.Ceiling => Mathf.CeilToInt(value),
                _ => throw new NotImplementedException(),
            };
        }

        public static bool InRange(this float value, float min, float max, bool inclusive = true)
        {
            if (inclusive)
                return value >= min && value <= max;
            else
                return value > min && value < max;
        }

        public static float PerformBinaryOperation(float left, float right, BinaryMathOperation operation)
        {
            return operation switch
            {
                BinaryMathOperation.Replace => right,
                BinaryMathOperation.Add => left + right,
                BinaryMathOperation.Subtract => left - right,
                BinaryMathOperation.Multiply => left * right,
                BinaryMathOperation.Divide => left / right,
                BinaryMathOperation.IntegerDivide => (int)(left / right),
                BinaryMathOperation.Modulo => left % right,
                BinaryMathOperation.Power => (float)Math.Pow(left, right),
                _ => throw new NotImplementedException(),
            };
        }

        //TODO: Remove after replacing all MathOperation with BinaryMathOperation in Processors & Obstacles
        public static BinaryMathOperation ToBinaryOperation(this MathOperation mathOperation)
            => (BinaryMathOperation)(mathOperation + 1);
    }
}
