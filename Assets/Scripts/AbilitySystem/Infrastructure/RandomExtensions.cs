using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.Infrastructure
{
    public static class RandomExtensions
    {
        public static bool TryChanceFraction(float chanceFraction) => IsRandomValueInRange(0, 1, chanceFraction);

        public static bool TryChancePercent(float chancePercent) => IsRandomValueInRange(0, 100, chancePercent);

        /// <summary>
        /// Generates random value between "minValue" and "maxValue" and returns true if it is less than or equals to "rangeThreshold".
        /// </summary>
        /// <param name="minValue"> Minimal random value. </param>
        /// <param name="maxValue"> Maximal random value. </param>
        /// <param name="rangeThreshold"> Threshold value after which random value will no longer fall in range. </param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static bool IsRandomValueInRange(float minValue, float maxValue, float rangeThreshold)
        {
            if (rangeThreshold < minValue || rangeThreshold > maxValue)
                throw new ArgumentException($"Range threshold value must be between {minValue} and {maxValue}.");
            if (float.IsNaN(rangeThreshold) || !float.IsFinite(rangeThreshold))
                throw new ArgumentException($"Range threshold value must be finite and not NaN.");
            return UnityEngine.Random.Range(minValue, maxValue) <= rangeThreshold;
        }
    }
}
