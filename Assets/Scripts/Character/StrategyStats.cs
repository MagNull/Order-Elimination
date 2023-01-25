using System;

namespace OrderElimination
{
    [Serializable]
    public struct StrategyStats
    {
        public int CostOfUpgrade;
        public int HealthGrowth;
        public int AttackGrowth;
        public int ArmorGrowth;
        public int EvasionGrowth;
        public int AccuracyGrowth;
    }
}