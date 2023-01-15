using System;
using System.Collections.Generic;
using CharacterAbility;
using CharacterAbility.BuffEffects;
using OrderElimination.Battle;
using UnityEngine;

namespace OrderElimination.BattleMap
{
    public class EnvironmentObject : IBattleObject
    {
        public event Action<Cell, Cell> Moved;
        public event Action<TakeDamageInfo> Damaged;
        private bool _isWalkable;
        private readonly ITickEffect[] _enterBuffs;
        private readonly BattleStats _stats;

        public IReadOnlyBattleStats Stats => _stats;
        public BattleObjectSide Side => _isWalkable ? BattleObjectSide.None : BattleObjectSide.Environment;
        public GameObject View { get; set; }

        public EnvironmentObject(ITickEffect[] enterBuffs, GameObject view, BattleStats stats, bool isWalkable)
        {
            View = view;
            _enterBuffs = enterBuffs;
            _stats = stats;
            _isWalkable = isWalkable;
        }

        public int GetAccuracyFrom(IBattleObject attacker)
        {
            throw new NotImplementedException();
        }

        public void OnEnter(IBattleObject battleObject)
        {
            Debug.Log(battleObject.View.name + " entered " + View.name);
            foreach (var buff in _enterBuffs) battleObject.AddTickEffect(buff);
            Debug.Log(battleObject.View.name + " entered " + View.name);
        }

        public void OnMoved(Cell from, Cell to)
        {
            throw new NotImplementedException();
        }

        public void OnLeave(IBattleObject battleObject)
        {
            foreach (var buff in _enterBuffs) battleObject.RemoveTickEffect(buff);
            Debug.Log(battleObject.View.name + " left " + View.name);
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            throw new NotImplementedException();
        }

        public void TakeRecover(int value, int accuracy, DamageHealTarget damageHealTarget)
        {
            throw new NotImplementedException();
        }

        public void AddTickEffect(ITickEffect effect)
        {
            throw new NotImplementedException();
        }

        public void RemoveTickEffect(ITickEffect effect)
        {
            throw new NotImplementedException();
        }

        public void ClearTickEffects()
        {
            throw new NotImplementedException();
        }

        public void ClearBuffEffects()
        {
            throw new NotImplementedException();
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