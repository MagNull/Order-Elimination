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

        //public static bool InRange(float value, float min, float max, bool inclusive = true)
        //{

        //}
    }
}
