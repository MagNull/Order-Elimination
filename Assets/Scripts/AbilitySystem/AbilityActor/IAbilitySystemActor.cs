using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class IAbilitySystemActor : IHaveBattleLifeStats, IEffectHolder, IMovable
    {
        public IAbilitySystemActor(
            IBattleMap battleMap, 
            BattleStats battleStats, 
            EntityType type, 
            BattleSide side, 
            AbilityData[] activeAbilities)//equipment
        {
            DeployedBattleMap = battleMap;
            _battleStats = battleStats;
            EntityType = type;
            BattleSide = side;
            foreach (var ability in activeAbilities)
            {
                ActiveAbilities.Add(new AbilityRunner(ability));
            }
            //foreach (var ability in passiveAbilities)
            //{
            //    PassiveAbilities.Add(new AbilityRunner(ability));
            //}
            foreach (var p in EnumExtensions.GetValues<ActionPoint>())
            {
                _actionPoints.Add(p, 0);
            }
        }

        #region IHaveLifeStats
        public IBattleLifeStats LifeStats => _battleStats;
        public event Action<DealtDamageInfo> Damaged;
        public event Action<HealRecoveryInfo> Healed;
        public DealtDamageInfo TakeDamage(DamageInfo damageInfo)
        {
            var dealtDamage = this.NoEventTakeDamage(damageInfo);
            Damaged?.Invoke(dealtDamage);
            return dealtDamage;
        }
        public HealRecoveryInfo TakeHeal(HealInfo healInfo)
        {
            var recoveryInfo = this.NoEventTakeHeal(healInfo);
            Healed?.Invoke(recoveryInfo);
            return recoveryInfo;
        }
        #endregion

        public EntityType EntityType { get; }
        public BattleSide BattleSide { get; }
        public IBattleStats BattleStats => _battleStats;

        public IBattleMap DeployedBattleMap { get; }
        public Vector2Int Position => DeployedBattleMap.GetPosition(this);
        public void Move(Vector2Int destination)//Task to wait for
        {
            var origin = DeployedBattleMap.GetPosition(this);
            //Wait for animations to finish
            DeployedBattleMap.RemoveEntity(this);
            DeployedBattleMap.PlaceEntity(this, destination);
            MovedFromTo?.Invoke(origin, destination);
        }
        public event Action<Vector2Int, Vector2Int> MovedFromTo;

        public IReadOnlyDictionary<ActionPoint, int> ActionPoints => _actionPoints;
        public void AddActionPoints(ActionPoint actionPoint, int value = 1)
        {
            if (value < 0) throw new ArgumentOutOfRangeException();
            if (!_actionPoints.ContainsKey(actionPoint)) _actionPoints.Add(actionPoint, 0);
            _actionPoints[actionPoint] += value;
        }
        public void RemoveActionPoints(ActionPoint actionPoint, int value = 1)
        {
            if (value < 0) throw new ArgumentOutOfRangeException();
            if (!_actionPoints.ContainsKey(actionPoint)) throw new KeyNotFoundException();
            if (_actionPoints[actionPoint] < value) throw new ArgumentOutOfRangeException();
            _actionPoints[actionPoint] -= value;

        }
        public void SetActionPoints(ActionPoint actionPoint, int value)
        {
            if (value < 0) throw new ArgumentOutOfRangeException();
            if (!_actionPoints.ContainsKey(actionPoint)) 
                _actionPoints.Add(actionPoint, 0);
            _actionPoints[actionPoint] = value;
        }
        public List<AbilityRunner> ActiveAbilities { get; } = new List<AbilityRunner>();
        public List<AbilityRunner> PassiveAbilities { get; } = new List<AbilityRunner>();
        //public bool UseAbility(Ability ability, CellTargetGroups targets); //TODO return AbilityUseContext

        public ActionProcessor ActionProcessor { get; }

        #region IEffectHolder
        public IEnumerable<IEffect> Effects => _effects;
        public bool HasEffect(IEffect effect) => _effects.Contains(effect);
        public bool CanApplyEffect(IEffect effect) => !HasEffect(effect) || effect.IsStackable;
        public event Action<IEffect> EffectAdded;
        public event Action<IEffect> EffectRemoved;

        public bool ApplyEffect(IEffect effect, IAbilitySystemActor applier)
        {
            if (CanApplyEffect(effect) && effect.Activate(this, applier))
            {
                _effects.Add(effect);
                return true;
            }
            return false;
        }

        public bool RemoveEffect(IEffect effect)
        {
            if (effect.CanBeForceRemoved && _effects.Contains(effect) && effect.Deactivate())
            {
                _effects.Remove(effect);
            }
            return false;
        }
        #endregion

        //IBattleObstacle?

        private readonly BattleStats _battleStats;
        private readonly HashSet<IEffect> _effects = new HashSet<IEffect>();
        private readonly Dictionary<ActionPoint, int> _actionPoints = new Dictionary<ActionPoint, int>();
    }

    public enum EntityType
    {
        Character,
        MapObject
    }
}
