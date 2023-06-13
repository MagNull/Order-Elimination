using OrderElimination.MetaGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination
{
    public static class SquadMediator
    {
        private static List<GameCharacter> _characterList;
        public static StrategyStats? Stats { get; private set; }
        public static List<GameCharacter> CharacterList => _characterList;

        public static void SetCharacters(List<GameCharacter> characters)
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
