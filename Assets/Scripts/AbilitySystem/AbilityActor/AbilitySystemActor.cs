using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class AbilitySystemActor : IHaveBattleLifeStats, IEffectHolder, IMovable
    {
        public AbilitySystemActor(
            IBattleContext battleContext, 
            BattleStats battleStats, 
            EntityType type, 
            BattleSide side, 
            ActiveAbilityData[] activeAbilities,
            PassiveAbilityData[] passiveAbilities)//equipment
        {
            BattleContext = battleContext;
            _battleStats = battleStats;
            battleStats.HealthDepleted -= OnHealthDepleted;
            battleStats.HealthDepleted += OnHealthDepleted;
            EntityType = type;
            BattleSide = side;
            foreach (var ability in activeAbilities)
            {
                ActiveAbilities.Add(new ActiveAbilityRunner(ability));
            }
            foreach (var ability in passiveAbilities)
            {
                PassiveAbilities.Add(new PassiveAbilityRunner(ability));
            }
            foreach (var p in EnumExtensions.GetValues<ActionPoint>())
            {
                _actionPoints.Add(p, 0);
            }
            _actionProcessor = new Lazy<EntityActionProcessor>(() => EntityActionProcessor.Create(this));

            void OnHealthDepleted(ILifeBattleStats lifeStats)
            {
                Died?.Invoke(this);
            }
        }

        public EntityType EntityType { get; }
        public BattleSide BattleSide { get; }
        public IBattleStats BattleStats => _battleStats;

        #region IHaveLifeStats
        public ILifeBattleStats LifeStats => _battleStats;
        public bool IsAlive => LifeStats.Health > 0;
        public event Action<DealtDamageInfo> Damaged;
        public event Action<HealRecoveryInfo> Healed;
        public event Action<AbilitySystemActor> Died;

        public DealtDamageInfo TakeDamage(DamageInfo damageInfo)
        {
            var dealtDamage = this.NoEventTakeDamage(damageInfo);
            Damaged?.Invoke(dealtDamage);
            if (!IsAlive) Died?.Invoke(this);
            return dealtDamage;
        }

        public HealRecoveryInfo TakeHeal(HealInfo healInfo)
        {
            var recoveryInfo = this.NoEventTakeHeal(healInfo);
            Healed?.Invoke(recoveryInfo);
            return recoveryInfo;
        }
        #endregion

        #region EntityMover
        public IBattleContext BattleContext { get; }
        public IBattleMap DeployedBattleMap => BattleContext.BattleMap;
        public Vector2Int Position => DeployedBattleMap.GetPosition(this);
        public bool Move(Vector2Int destination, bool forceMove = false)
        {
            if (StatusHolder.HasStatus(BattleStatus.CantMove) && !forceMove)
                return false;
            var origin = Position;
            DeployedBattleMap.PlaceEntity(this, destination);
            MovedFromTo?.Invoke(origin, destination);
            return true;
        }

        public event Action<Vector2Int, Vector2Int> MovedFromTo;
        #endregion

        #region AbilityCaster
        private readonly Dictionary<ActionPoint, int> _actionPoints = new Dictionary<ActionPoint, int>();

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
            if (_actionPoints[actionPoint] < value) throw new ArgumentOutOfRangeException("Entity doesn't have enough points to be removed.");
            _actionPoints[actionPoint] -= value;
        }
        public void RemoveActionPoints(IReadOnlyDictionary<ActionPoint, int> actionPoints)
        {
            foreach (var point in actionPoints.Keys)
            {
                if (ActionPoints[point] < actionPoints[point])
                    throw new ArgumentOutOfRangeException("Entity doesn't have enough points to be removed.");
            }
            foreach (var point in actionPoints.Keys)
            {
                RemoveActionPoints(point, actionPoints[point]);
            }
        }
        public void SetActionPoints(ActionPoint actionPoint, int value)
        {
            if (value < 0) throw new ArgumentOutOfRangeException();
            if (!_actionPoints.ContainsKey(actionPoint)) 
                _actionPoints.Add(actionPoint, 0);
            _actionPoints[actionPoint] = value;
        }
        public List<ActiveAbilityRunner> ActiveAbilities { get; } = new();
        public List<PassiveAbilityRunner> PassiveAbilities { get; } = new();
        public bool IsBusy { get; set; } //Performs ability
        #endregion

        #region IEffectHolder
        private readonly HashSet<BattleEffect> _effects = new HashSet<BattleEffect>();

        public IEnumerable<BattleEffect> Effects => _effects;
        public bool HasEffect(IEffectData effect) => _effects.Any(e => e.EffectData == effect);
        public event Action<BattleEffect> EffectAdded;
        public event Action<BattleEffect> EffectRemoved;

        public bool ApplyEffect(IEffectData effect, AbilitySystemActor applier, out BattleEffect appliedEffect)
        {
            var battleEffect = new BattleEffect(effect, BattleContext);
            if (effect.CanBeAppliedOn(this) && battleEffect.Apply(this, applier))
            {
                _effects.Add(battleEffect);
                battleEffect.Deactivated += OnEffectDeactivated;
                EffectAdded?.Invoke(battleEffect);
                battleEffect.Activate();
                appliedEffect = battleEffect;
                return true;
            }
            appliedEffect = null;
            return false;
        }

        public bool RemoveEffect(BattleEffect effect)
        {
            if (_effects.Contains(effect) && effect.TryDeactivate())
            {
                OnEffectDeactivated(effect);
                return true;
            }
            return false;
        }

        private void OnEffectDeactivated(BattleEffect effect)
        {
            effect.Deactivated -= OnEffectDeactivated;
            _effects.Remove(effect);
            EffectRemoved?.Invoke(effect);
        }
        #endregion

        public EntityStatusHolder StatusHolder { get; } = new EntityStatusHolder();

        public EntityActionProcessor ActionProcessor => _actionProcessor.Value;

        //IBattleObstacle?

        private readonly Lazy<EntityActionProcessor> _actionProcessor;
        private readonly BattleStats _battleStats;
    }
}
