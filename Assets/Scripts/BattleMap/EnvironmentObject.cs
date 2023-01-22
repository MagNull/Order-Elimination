using System;
using System.Collections.Generic;
using System.Linq;
using CharacterAbility;
using CharacterAbility.BuffEffects;
using OrderElimination.Battle;
using UnityEngine;

namespace OrderElimination.BM
{
    [Serializable]
    public class EnvironmentObject : IBattleObject, IEquatable<EnvironmentObject>
    {
        public event Action<Cell, Cell> Moved;
        public event Action<TakeDamageInfo> Damaged;
        public event Action Destroyed;

        [SerializeField]
        private bool _isWalkable;
        private readonly BattleMap _map;
        [SerializeReference]
        private ITickEffect[] _enterBuffs;
        private readonly BattleStats _stats;
        private int _lifeTime;

        public IReadOnlyList<ITickEffect> AllEffects => new List<ITickEffect>();
        public IReadOnlyBattleStats Stats => _stats;
        public BattleObjectSide Side => _isWalkable ? BattleObjectSide.Environment : BattleObjectSide.Obstacle;

        public IBattleObjectView View { get; set; }

        public EnvironmentObject(ITickEffect[] enterBuffs, IBattleObjectView view, BattleStats stats, bool isWalkable,
            BattleMap map,
            int lifeTime = 999)
        {
            View = view;
            _enterBuffs = enterBuffs;
            _stats = stats;
            _isWalkable = isWalkable;
            _map = map;
            _lifeTime = lifeTime;
        }

        public int GetAccuracyFrom(IBattleObject attacker)
        {
            return 100;
        }

        public void OnEnter(IBattleObject battleObject)
        {
            foreach (var buff in _enterBuffs) battleObject.AddTickEffect(buff);
        }

        public void OnMoved(Cell from, Cell to)
        {
        }

        public void OnLeave(IBattleObject battleObject)
        {
            foreach (var buff in _enterBuffs) buff.RemoveTickEffect(battleObject);
        }

        public TakeDamageInfo TakeDamage(DamageInfo damageInfo)
        {
            return new TakeDamageInfo();
        }

        public void TakeRecover(int value, int accuracy, DamageHealTarget damageHealTarget)
        {
        }

        public void AddTickEffect(ITickEffect effect)
        {
        }

        public void RemoveTickEffect(ITickEffect effect)
        {
        }

        public void ClearTickEffects()
        {
        }

        public void ClearBuffEffects()
        {
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

        public void OnRoundStart()
        {
            _lifeTime--;

            if (_lifeTime > 0)
                return;

            Destroy();
            Destroyed?.Invoke();
        }

        private void Destroy()
        {
            var cellObj = _map.GetCell(this).GetObject();
            var cellObjCoord = _map.GetCoordinate(cellObj);
            _map.DestroyObject(this);
            if (cellObj == this)
                return;

            OnLeave(cellObj);
            _map.MoveTo(cellObj, cellObjCoord.x, cellObjCoord.y);
        }

        public bool Equals(EnvironmentObject other)
        {
            return other is not null && other.Side == Side &&
                   other._enterBuffs.All(ef => _enterBuffs.Any(e2 => e2.Equals(ef)));
        }
    }
}