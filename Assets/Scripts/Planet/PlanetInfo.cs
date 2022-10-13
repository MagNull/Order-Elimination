using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination
{
    public class PlanetInfo
    {
        private Squad _squad;
        private Squad _opponents;

        public int сhanceOfItems { get; private set; }
        public int chanceOfFighting {get; private set;}
        public int chanceOfFightingBack { get; private set; }
        public float expirience { get; private set; }
        
        public PlanetInfo(Squad squad, Squad opponents)
        {
            _squad = squad;
            _opponents = opponents;
        }

        public void MoveSquad(Vector2Int position)
        {
            _squad.Move(position);
        }

        public void AddOpponent(Character character)
        {
            _opponents.AddCharacter(character);
        }

        public void RemoveOpponent(Character character)
        {
            _opponents.RemoveCharacter(character);
        }
    }
}