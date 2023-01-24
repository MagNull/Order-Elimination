using System;

namespace OrderElimination
{
    [Serializable]
    public struct StrategyStats
    {
        public int CostOfUpgrade;
        public readonly int HealthGrowth;
        public readonly int ArmorGrowth;
        public readonly int AttackGrowth;
        public readonly int AccuracyGrowth;
        public readonly int EvasionGrowth;
    }
}