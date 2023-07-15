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

        public IContextValueGetter Clone()
        {
            var clone = new RandomValueGetter();
            clone.RangeStart = RangeStart.Clone();
            clone.RangeEnd = RangeEnd.Clone();
            clone.RoundToInt = RoundToInt;
            return clone;
        }

        public float GetValue(ActionContext useContext)
        {
            var rand = Random.Range(RangeStart.GetValue(useContext), RangeEnd.GetValue(useContext));
            return RoundToInt ? Mathf.RoundToInt(rand) : rand;
        }
    }
}
