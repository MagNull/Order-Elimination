using System;
using CharacterAbility.BuffEffects;
using OrderElimination.Battle;
using UnityEngine;

namespace OrderElimination.BattleMap
{
    public class NullBattleObject : IBattleObject
    {
        public event Action<int, int, DamageCancelType> Damaged;
        public event Action Died;

        public void TakeDamage(int damage, int accuracy, DamageHealType damageHealType)
        {
            Debug.LogWarning("Try Damage Empty Object");
        }

        public BattleObjectSide Side => BattleObjectSide.None;

        public IReadOnlyBattleStats Stats { get; }

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
        }

        public void RemoveTickEffect(ITickEffect effect)
        { 
            Debug.LogError("Try remove tick effect from null battle object");
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

        public void AddBuffEffect(IStatsBuffEffect statsBuffEffect)
        {
            Debug.LogWarning("Try add buff effect to null battle object");
        }

        public void RemoveBuffEffect(IStatsBuffEffect statsBuffEffect)
        {
            Debug.LogWarning("Try remove buff effect from null battle object");
        }

        public void ClearBuffEffects()
        {
            Debug.LogWarning("Try clear buff effects from null battle object");
        }
    }
}