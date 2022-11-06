using System;
using UnityEngine;

namespace OrderElimination.BattleMap
{
    public class NullBattleObject : IBattleObject
    {
        public BattleObjectSide Side => BattleObjectSide.None;

        public GameObject GetView()
        {
            Debug.LogError("Try get view form null battle object");
            throw new NullReferenceException();
        }

        public void TakeDamage(int damage, int accuracy)
        {
            Debug.LogError("Try take damage from null battle object");
            throw new NullReferenceException();
        }

        public void TakeHeal(int heal, int accuracy)
        { 
            Debug.LogError("Try take heal from null battle object");
            throw new NullReferenceException();
        }

        public void AddTickEffect(ITickEffect effect)
        { 
            Debug.LogError("Try add tick effect to null battle object");
            throw new NullReferenceException();
        }

        public void RemoveTickEffect(ITickEffect effect)
        { 
            Debug.LogError("Try remove tick effect from null battle object");
            throw new NullReferenceException();
        }

        public void OnTurnStart()
        { 
            Debug.LogError("Try call OnTurnStart from null battle object");
            throw new NullReferenceException();
        }
    }
}