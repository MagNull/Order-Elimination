using System;
using System.Collections.Generic;
using CharacterAbility;
using CharacterAbility.BuffEffects;
using OrderElimination.Battle;
using UnityEngine;

namespace OrderElimination.BM
{
    public class NullBattleObject : IBattleObject
    {
        public event Action<TakeDamageInfo> Damaged;

        public event Action<Cell, Cell> Moved;

        public event Action Died;

        public IReadOnlyList<ITickEffect> AllEffects => new List<ITickEffect>();

        public BattleObjectSide Side => BattleObjectSide.None;
        public bool IsAlive => true;

        public IBattleObjectView View
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

        public int GetAccuracyFrom(IBattleObject attacker)
        {
            return 0;
        }

        public IReadOnlyBattleStats Stats => default(BattleStats);
        
        
        public void OnMoved(Cell from, Cell to)
        {
            Debug.LogWarning("Try Move Empty Object");
        }

        public void TakeRecover(int value, int accuracy, DamageHealTarget damageHealTarget)
        { 
            Debug.LogWarning("Try take heal from null battle object");
        }

        public void AddTickEffect(ITickEffect effect)
        { 
            Debug.LogWarning("Try add tick effect to null battle object");
        }

        public void RemoveTickEffect(ITickEffect effect)
        { 
            Debug.LogWarning("Try remove tick effect from null battle object");
        }

        public TakeDamageInfo TakeDamage(DamageInfo damageInfo)
        {
            Debug.LogWarning("Try Damage Empty Object");
            return new TakeDamageInfo();
        }

        public void ClearTickEffects()
        {
            throw new NotImplementedException();
        }

        public void OnTurnStart()
        { 
            Debug.LogWarning("Try call OnTurnStart from null battle object");
        }

        public void AddBuffEffect(StatsBuffEffect statsBuffEffect)
        {
            Debug.LogWarning("Try add buff effect to null battle object");
        }

        public void RemoveBuffEffect(StatsBuffEffect statsBuffEffect)
        {
            Debug.LogWarning("Try remove buff effect from null battle object");
        }

        public void ClearBuffEffects()
        {
            Debug.LogWarning("Try clear buff effects from null battle object");
        }

        public IReadOnlyList<ITickEffect> GetTickEffects(ITickEffect tickEffectType)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<StatsBuffEffect> GetTickEffects(StatsBuffEffect tickEffectType)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<IncomingBuff> GetTickEffects(IncomingBuff tickEffectType)
        {
            throw new NotImplementedException();
        }
    }
}