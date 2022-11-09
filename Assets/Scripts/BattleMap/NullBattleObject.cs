using System;
using UnityEngine;

namespace OrderElimination.BattleMap
{
    public class NullBattleObject : IBattleObject
    {
        public event Action<int> Damaged;
        public void TakeDamage(int damage, int accuracy, DamageHealType damageHealType)
        {
            throw new NotImplementedException();
        }

        public BattleObjectSide Side => BattleObjectSide.None;

        public GameObject GetView()
        {
            Debug.LogError("Try get view form null battle object");
            throw new NullReferenceException();
        }

        public void TakeRecover(int value, int accuracy, DamageHealType damageHealType)
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

        public void ClearTickEffects()
        {
            throw new NotImplementedException();
        }

        public void OnTurnStart()
        { 
            Debug.LogError("Try call OnTurnStart from null battle object");
            throw new NullReferenceException();
        }
    }
}