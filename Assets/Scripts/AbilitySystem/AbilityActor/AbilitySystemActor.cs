using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class AbilitySystemActor : IHaveBattleLifeStats, IEffectHolder, IMovable, IBattleDisposable
    {
        private readonly Lazy<EntityActionProcessor> _actionProcessor;
        private readonly Lazy<BattleObstacle> _obstacle;
        private readonly BattleStats _battleStats;
        public bool IsDisposedFromBattle { get; private set; } = false;

        public AbilitySystemActor(
            IBattleContext battleContext, 
            BattleStats battleStats, 
            EntityType type, 
            BattleSide side,
            IActiveAbilityData[] activeAbilities,
            IPassiveAbilityData[] passiveAbilities,
            IBattleObstacleSetup obstacleSetup)//equipment
        {
            BattleContext = battleContext;
            DeployedBattleMap = BattleContext.BattleMap;
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
            _obstacle = new Lazy<BattleObstacle>(() => new BattleObstacle(obstacleSetup, this));

            void OnHealthDepleted(ILifeBattleStats lifeStats)
            {
                //IsAlive = false;
            }
        }

        public EntityType EntityType { get; }
        public BattleSide BattleSide { get; }
        public IBattleStats BattleStats => _battleStats;
        public IBattleContext BattleContext { get; }
        public IBattleMap DeployedBattleMap { get; private set; }
        //public BattleEntityView GetEntityView() => IsDisposedFromBattle ? null : BattleContext.EntitiesBank.GetViewByEntity(this);

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
            if (!IsAlive) OnDeath();
            return dealtDamage;
        }

        public HealRecoveryInfo TakeHeal(HealInfo healInfo)
        {
            var recoveryInfo = this.NoEventTakeHeal(healInfo);
            Healed?.Invoke(recoveryInfo);
            return recoveryInfo;
        }

        private void OnDeath()
        {
            if (IsAlive) return;//throw new InvalidOperationException("Entity is alive.");
            Died?.Invoke(this);
            DisposeFromBattle();
        }
        #endregion

        #region EntityMover
        public Vector2Int Position => DeployedBattleMap.GetPosition(this);
        public bool CanMove => !StatusHolder.HasStatus(BattleStatus.CantMove);

        public bool Move(Vector2Int destination, bool forceMove = false)
        {
            if (!CanMove && !forceMove)
                return false;
            if (IsDisposedFromBattle)
                return false;
            var origin = Position;
            DeployedBattleMap.PlaceEntity(this, destination);
            MovedFromTo?.Invoke(origin, destination);
            return true;
        }

        public event Action<Vector2Int, Vector2Int> MovedFromTo;
        #endregion

        #region AbilityCaster
        private readonly Dictionary<ActionPoint, int> _actionPoints = new();

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
        public bool IsPerformingAbility { get; set; } //Performs ability
        #endregion

        #region IEffectHolder
        private readonly HashSet<BattleEffect> _effects = new HashSet<BattleEffect>();

        public IEnumerable<BattleEffect> Effects => _effects;
        public bool HasEffect(IEffectData effect) => _effects.Any(e => e.EffectData == effect);
        public event Action<BattleEffect> EffectAdded;
        public event Action<BattleEffect> EffectRemoved;

        public bool ApplyEffect(IEffectData effectData, AbilitySystemActor applier, out BattleEffect appliedEffect)
        {
            var effect = new BattleEffect(effectData, BattleContext);
            if (effectData.CanBeAppliedOn(this) && effect.Apply(this, applier))
            {
                _effects.Add(effect);
                effect.Deactivated += OnEffectDeactivated;
                EffectAdded?.Invoke(effect);
                effect.Activate();
                appliedEffect = effect;
                return true;
            }
            appliedEffect = null;
            return false;
        }

        public bool RemoveEffect(BattleEffect effect)
        {
            return _effects.Contains(effect) && effect.TryDeactivate();
        }

        private void OnEffectDeactivated(BattleEffect effect)
        {
            effect.Deactivated -= OnEffectDeactivated;
            _effects.Remove(effect);
            EffectRemoved?.Invoke(effect);
        }
        #endregion

        #region IBattleDisposable
        public event Action<IBattleDisposable> DisposedFromBattle;

        public bool DisposeFromBattle()
        {
            if (IsDisposedFromBattle)
                return false;
            //Dispose
            foreach (var passiveAbility in PassiveAbilities)
            {
                passiveAbility.Deactivate();
            }
            var effects = _effects.ToArray();
            foreach (var effect in effects)
            {
                effect.TryDeactivate();
            }
            DeployedBattleMap.RemoveEntity(this);
            //DeployedBattleMap = null;
            IsDisposedFromBattle = true;
            DisposedFromBattle?.Invoke(this);
            return true;
        }
        #endregion

        public EntityStatusHolder StatusHolder { get; } = new EntityStatusHolder();

        public EntityActionProcessor ActionProcessor => _actionProcessor.Value;

        public BattleObstacle Obstacle => _obstacle.Value;
    }
}
