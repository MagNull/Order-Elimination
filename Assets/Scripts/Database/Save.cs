using System;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination
{
    [Serializable]
    public class Save
    {
        public int CountMove;
        public Vector3 EnemyPosition;
        public List<bool> IsMoveSquads;
        public List<Vector3> SquadPositions;
        public List<CharacterStats> CharacterStats;
        public int Money;

        public Save()
        {
            CountMove = 0;
            EnemyPosition = Vector3.zero;
            IsMoveSquads = new List<bool> { false, false };
            SquadPositions = new List<Vector3> { new Vector3(50, 110, 0), new Vector3(150, 110, 0) };
            Money = 0;
            CharacterStats = new List<CharacterStats>();
        }

        public Save(int countMove, Vector3 enemyPosition, List<bool> isMoveSquads, 
            List<Vector3> squadPositions, int money, List<CharacterStats> stats)
        {
            CountMove = countMove;
            EnemyPosition = enemyPosition;
            IsMoveSquads = isMoveSquads;
            SquadPositions = squadPositions;
            Money = money;
            CharacterStats = stats;
        }
    }
}