using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.Infrastructure
{
    //TODO Значение в процентах может быть больше 100%, если его не обрезать.
    //В таком случае нужно убрать исключение для значений больше 1 (>100%).
    //Аналогично для отрицательных значений.
    public static class RandomExtensions
    {
        /// <summary>
        /// Returns true if random value between 0 and 1 is less than or equals to given chanceFraction.
        /// </summary>
        /// <param name="chanceFraction"> Value between 0 and 1. </param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static bool TryChance(float chanceFraction) => IsRandomValueInRange(0, 1, chanceFraction);

        //public static bool TryChanceInPercent(float chancePercent) => IsRandomValueInRange(0, 100, chancePercent);

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
            //if (rangeThreshold < minValue || rangeThreshold > maxValue)
            //    throw new ArgumentException($"Range threshold value must be between {minValue} and {maxValue}.");
            if (float.IsNaN(rangeThreshold) || !float.IsFinite(rangeThreshold))
                throw new ArgumentException($"Range threshold value must be finite and not NaN.");
            return UnityEngine.Random.Range(minValue, maxValue) <= rangeThreshold;
        }
    }
}
