using OrderElimination.MacroGame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination
{
    public static class SquadMediator
    {
        private static List<GameCharacter> _characterList;
        public static StrategyStats? PlayerSquadStats { get; private set; }
        public static List<GameCharacter> CharacterList => _characterList;

        public static void SetCharacters(List<GameCharacter> characters)
        {
            _characterList = characters;
        }

        public static void SetStatsCoefficient(StrategyStats stats)
        {
            PlayerSquadStats = stats;
        }
    }   
}
