using Sirenix.Serialization;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OrderElimination.AbilitySystem
{
    [Serializable]
    public struct RandomValueGetter : IContextValueGetter
    {
        [OdinSerialize]
        public IContextValueGetter RangeStart { get; private set; }

        [OdinSerialize]
        public IContextValueGetter RangeEnd { get; private set; }

        [OdinSerialize]
        public bool RoundToInt { get; private set; }

        public string DisplayedFormula
        {
            get
            {
                var start = RangeStart != null 
                    ? RangeStart.DisplayedFormula 
                    : IContextValueGetter.EmptyValueReplacement;
                var end = RangeEnd != null 
                    ? RangeEnd.DisplayedFormula 
                    : IContextValueGetter.EmptyValueReplacement;
                return $"[{start};{end}]";
            }
        }

        public bool CanBePrecalculatedWith(ValueCalculationContext context)
        {
            return false;//for precalculation
            return true;//for calculation
        }

        public IContextValueGetter Clone()
        {
            var clone = new RandomValueGetter();
            clone.RangeStart = RangeStart.Clone();
            clone.RangeEnd = RangeEnd.Clone();
            clone.RoundToInt = RoundToInt;
            return clone;
        }

        public float GetValue(ValueCalculationContext context)
        {
            var rand = Random.Range(RangeStart.GetValue(context), RangeEnd.GetValue(context));
            return RoundToInt ? Mathf.RoundToInt(rand) : rand;
        }
    }
}
