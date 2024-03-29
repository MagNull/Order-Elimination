﻿using System;

namespace OrderElimination.Infrastructure
{
    public static class MathStringExtensions
    {
        public static string AsString(this BinaryComparisonOperation operation)
        {
            return operation switch
            {
                BinaryComparisonOperation.Equals => "==",
                BinaryComparisonOperation.NotEquals => "!=",
                BinaryComparisonOperation.GreaterThan => ">",
                BinaryComparisonOperation.LessThan => "<",
                BinaryComparisonOperation.GreaterOrEquals => ">=",
                BinaryComparisonOperation.LessOrEquals => "<=",
                _ => throw new NotImplementedException(),
            };
        }

        public static string AsString(this BinaryMathOperation operation)
        {
            return operation switch
            {
                BinaryMathOperation.Add => "+",
                BinaryMathOperation.Subtract => "-",
                BinaryMathOperation.Multiply => "x",
                BinaryMathOperation.Divide => "/",
                BinaryMathOperation.IntegerDivide => "//",
                BinaryMathOperation.Modulo => "%",
                BinaryMathOperation.Power => "^",
                _ => throw new NotImplementedException(),
            };
        }
    }
}
