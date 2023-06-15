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

        public BattleObjectType Type => BattleObjectType.None;
        public bool IsAlive => true;

        public IBattleObjectView View
        {
            get
            {
                Logging.LogError("Try get view for null battle object");
                Logging.LogException( new NullReferenceException());
                throw new NullReferenceException();
            }
            set
            {
                Logging.LogError("Try set view form null battle object");
                Logging.LogException( new NullReferenceException());
                throw new NullReferenceException();
            }
        }

        public float GetAccuracyFrom(IBattleObject attacker)
        {
            return 0;
        }

        public IReadOnlyBattleStats Stats => default(OldBattleStats);
        
        
        public void OnMoved(Cell from, Cell to)
        {
            Logging.LogWarning("Try Move Empty Object");
        }

        public void TakeRecover(float value, float accuracy, DamageHealTarget damageHealTarget)
        { 
            Logging.LogWarning("Try take heal from null battle object");
        }

        public void AddTickEffect(ITickEffect effect)
        { 
            Logging.LogWarning("Try add tick effect to null battle object");
        }

        public void RemoveTickEffect(ITickEffect effect)
        { 
            Logging.LogWarning("Try remove tick effect from null battle object");
        }

        public TakeDamageInfo TakeDamage(DamageInfo damageInfo)
        {
            Logging.LogWarning("Try Damage Empty Object");
            return new TakeDamageInfo();
        }

        public void ClearTickEffects()
        {
            Logging.LogException( new NotImplementedException());
        }

        public void OnTurnStart()
        { 
            Logging.LogWarning("Try call OnTurnStart from null battle object");
        }

        public void AddBuffEffect(StatsBuffEffect statsBuffEffect)
        {
            Logging.LogWarning("Try add buff effect to null battle object");
        }

        public void RemoveBuffEffect(StatsBuffEffect statsBuffEffect)
        {
            Logging.LogWarning("Try remove buff effect from null battle object");
        }

        public void ClearBuffEffects()
        {
            Logging.LogWarning("Try clear buff effects from null battle object");
        }

        public IReadOnlyList<ITickEffect> GetTickEffects(ITickEffect tickEffectType)
        {
            Logging.LogException( new NotImplementedException());
            throw new NotImplementedException();
        }

        public IReadOnlyList<StatsBuffEffect> GetTickEffects(StatsBuffEffect tickEffectType)
        {
            Logging.LogException( new NotImplementedException());
            throw new NotImplementedException();
        }

        public IReadOnlyList<IncomingBuff> GetTickEffects(IncomingBuff tickEffectType)
        {
            Logging.LogException( new NotImplementedException());
            throw new NotImplementedException();
        }
    }
}