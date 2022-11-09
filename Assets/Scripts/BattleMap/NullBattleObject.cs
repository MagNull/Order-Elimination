using System;
using CharacterAbility.BuffEffects;
using UnityEngine;

namespace OrderElimination.BattleMap
{
    public class NullBattleObject : IBattleObject
    {
        public event Action<int, int> Damaged;
        public event Action Died;

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

        public void ClearOverEffects()
        {
            throw new NotImplementedException();
        }

        public void OnTurnStart()
        { 
            Debug.LogError("Try call OnTurnStart from null battle object");
            throw new NullReferenceException();
        }

        public IReadOnlyBattleStats Stats { get; }
        public void AddBuffEffect(IStatsBuffEffect statsBuffEffect)
        {
            throw new NotImplementedException();
        }

        public void RemoveBuffEffect(IStatsBuffEffect statsBuffEffect)
        {
            throw new NotImplementedException();
        }

        public void ClearBuffEffects()
        {
            throw new NotImplementedException();
        }
    }
}