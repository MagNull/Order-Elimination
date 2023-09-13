using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace OrderElimination.Infrastructure.Maths
{
    public struct FloatRange
    {
        public FloatRange(float start, float end)
        {
            Start = start;
            End = end;
        }

        [HorizontalGroup("Range")]
        [SerializeField]
        public float Start { get; set; }

        [HorizontalGroup("Range")]
        [SerializeField]
        public float End { get; set; }

        public float MinValue => Math.Min(Start, End);
        public float MaxValue => Math.Max(Start, End);

        public bool ContainsInclusive(float value)
        {
            if (Start == End) return value == Start;
            return Start < End
                ? Start <= value && value <= End
                : End <= value && value <= Start;
        }

        public bool ContainsExclusive(float value)
        {
            if (Start == End) return false;
            return Start < End
                ? Start < value && value < End
                : End < value && value < Start;
        }

        //public static bool operator <(float value, FloatRange range)
        //    => value < range.MinValue;
        //public static bool operator >(float value, FloatRange range)
        //    => value > range.MaxValue;
        //public static bool operator <(FloatRange range, float value)
        //    => value > range;
        //public static bool operator >(FloatRange range, float value)
        //    => value < range;

        //public static bool operator <=(float value, FloatRange range)
        //    => value <= range.MinValue;
        //public static bool operator >=(float value, FloatRange range)
        //    => value >= range.MaxValue;
        //public static bool operator <=(FloatRange range, float value)
        //    => value >= range;
        //public static bool operator >=(FloatRange range, float value)
        //    => value <= range;

        public override string ToString()
            => $"[{Start}; {End}]";
    }
}
