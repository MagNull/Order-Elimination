using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination
{
    public static class SquadMediator
    {
        private static List<Character> _characterList;
        public static StrategyStats Stats { get; private set; }
        public static IReadOnlyList<Character> CharacterList => _characterList;

        public static void SetCharacters(List<Character> characters)
        {
            _characterList = characters;
        }

        public static void SetStatsCoefficient(List<int> stats)
        {
            Stats = new StrategyStats()
            {
                HealthGrowth = stats[0],
                AttackGrowth = stats[1],
                ArmorGrowth = stats[2],
                EvasionGrowth = stats[3],
                AccuracyGrowth = stats[4]
            };
        }
    }   
}
