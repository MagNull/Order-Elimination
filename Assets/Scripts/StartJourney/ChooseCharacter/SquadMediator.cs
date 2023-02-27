using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination
{
    public static class SquadMediator
    {
        private static List<Character> _characterList;
        public static IReadOnlyList<Character> CharacterList => _characterList;

        public static void SetCharacters(List<Character> characters)
        {
            _characterList = characters;
        }
    }   
}
