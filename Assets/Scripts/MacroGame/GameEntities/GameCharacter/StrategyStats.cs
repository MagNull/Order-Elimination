using System;
using System.Text;

namespace OrderElimination
{
    [Serializable]
    public struct StrategyStats
    {
        public int HealthGrowth;
        public int ArmorGrowth;
        public int AttackGrowth;
        public int EvasionGrowth;
        public int AccuracyGrowth;

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"{nameof(HealthGrowth)}: {HealthGrowth}");
            builder.AppendLine($"{nameof(ArmorGrowth)}: {ArmorGrowth}");
            builder.AppendLine($"{nameof(AttackGrowth)}: {AttackGrowth}");
            builder.AppendLine($"{nameof(EvasionGrowth)}: {EvasionGrowth}");
            builder.AppendLine($"{nameof(AccuracyGrowth)}: {AccuracyGrowth}");
            return builder.ToString();
        }
    }
}