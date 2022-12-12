using System;
using CharacterAbility.BuffEffects;
using OrderElimination.Battle;
using UnityEngine;

namespace OrderElimination.BattleMap
{
    public class NullBattleObject : IBattleObject
    {
        public event Action<TakeDamageInfo> Damaged;

        public event Action<Cell, Cell> Moved;

        public event Action Died;

        public BattleObjectSide Side => BattleObjectSide.None;

        public GameObject View
        {
            get
            {
                Debug.LogError("Try get view for null battle object");
                throw new NullReferenceException();
            }
            set
            {
                Debug.LogError("Try set view form null battle object");
                throw new NullReferenceException();
            }
        }

        public IReadOnlyBattleStats Stats => default(BattleStats);
        
        
        public void OnMoved(Cell from, Cell to)
        {
            Debug.LogWarning("Try Move Empty Object");
        }

        public void TakeRecover(int value, int accuracy, DamageHealTarget damageHealTarget)
        { 
            Debug.LogError("Try take heal from null battle object");
            throw new NullReferenceException();
        }

        public void AddTickEffect(ITickEffect effect)
        { 
            Debug.LogWarning("Try add tick effect to null battle object");
        }

        public void RemoveTickEffect(ITickEffect effect)
        { 
            Debug.LogWarning("Try remove tick effect from null battle object");
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            Debug.LogWarning("Try Damage Empty Object");
        }

        public void ClearTickEffects()
        {
            throw new NotImplementedException();
        }

        public void OnTurnStart()
        { 
            Debug.LogWarning("Try call OnTurnStart from null battle object");
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