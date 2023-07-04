using System;
using UnityEngine;

namespace OrderElimination.Infrastructure
{
    public static class MathExtensions
    {
        public static float Round(float value, RoundingOption mode)
        {
            return mode switch
            {
                RoundingOption.Math => (float)Math.Round(value, MidpointRounding.AwayFromZero),
                RoundingOption.Floor => Mathf.Floor(value),
                RoundingOption.Ceiling => Mathf.Ceil(value),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
