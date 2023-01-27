using System;
using System.Collections.Generic;

namespace OrderElimination
{
    [Serializable]
    public class CharacterStats
    {
        public List<StrategyStats> Stats;

        public CharacterStats(List<StrategyStats> stats)
        {
            Stats = stats;
        }
    }
}