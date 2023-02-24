using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination
{
    public class SquadMediator : MonoBehaviour
    {
        private List<Character> _characterList;
        public List<Character> CharacterList => _characterList; 
        public void SetCharacters(List<Character> characters)
        {
            _characterList = characters;
        }
    }   
}
