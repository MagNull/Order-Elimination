using System;
using UnityEngine;

namespace OrderElimination.BattleMap
{
    public class NullBattleObject : IBattleObject
    {
        public GameObject GetView()
        {
            Debug.LogError("Try get view form null battle object");
            throw new NullReferenceException();
        }

        public void OnTurnStart()
        {
            
        }
    }
}