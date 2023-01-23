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
        public int Money;

        public Save()
        {
            CountMove = 0;
            EnemyPosition = Vector3.zero;
            IsMoveSquads = new List<bool> { false, false };
            SquadPositions = new List<Vector3> { new Vector3(50, 110, 0), new Vector3(150, 110, 0) };
            Money = 0;
        }

        public Save(int countMove, Vector3 enemyPosition, List<bool> isMoveSquads, 
            List<Vector3> squadPositions, int money)
        {
            CountMove = countMove;
            EnemyPosition = enemyPosition;
            IsMoveSquads = isMoveSquads;
            SquadPositions = squadPositions;
            Money = money;
        }
    }
}