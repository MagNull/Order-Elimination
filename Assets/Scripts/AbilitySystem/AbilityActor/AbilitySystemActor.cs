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
            IBattleObstacleSetup obstacleSetup)//equipment
        {
            BattleContext = battleContext;
            DeployedBattleMap = BattleContext.BattleMap;
            _battleStats = battleStats;
            battleStats.HealthDepleted -= OnHealthDepleted;
            battleStats.HealthDepleted += OnHealthDepleted;
            EntityType = type;
            BattleSide = side;
            foreach (var p in EnumExtensions.GetValues<ActionPoint>())
            {
                _actionPoints.Add(p, 0);
            }
            _actionProcessor = new Lazy<EntityActionProcessor>(() => EntityActionProcessor.Create(this));
            _obstacle = new Lazy<BattleObstacle>(() => new BattleObstacle(obstacleSetup, this));

            void OnHealthDepleted(IBattleLifeStats lifeStats)
            {
                //IsAlive = false;
            }
        }

        public EntityType EntityType { get; }
        public BattleSide BattleSide { get; }
        public IBattleLifeStats BattleStats => _battleStats;
        public IBattleContext BattleContext { get; }
        public IBattleMap DeployedBattleMap { get; private set; }
        //public BattleEntityView GetEntityView() => IsDisposedFromBattle ? null : BattleContext.EntitiesBank.GetViewByEntity(this);

        #region IHaveLifeStats
        public bool IsAlive => BattleStats.Health > 0;
        public event Action<DealtDamageInfo> Damaged;
        public event Action<DealtRecoveryInfo> Healed;
        public event Action<AbilitySystemActor> Died;

        public DealtDamageInfo TakeDamage(DamageInfo incomingDamage)
        {
            var dealtDamage = IBattleLifeStats.DistributeDamage(BattleStats, incomingDamage);
            BattleStats.TotalArmor -= dealtDamage.TotalArmorDamage;
            BattleStats.Health -= dealtDamage.TotalHealthDamage;
            Damaged?.Invoke(dealtDamage);
            if (!IsAlive) OnDeath();
            return dealtDamage;
        }

        public DealtRecoveryInfo TakeRecovery(RecoveryInfo incomingHeal)
        {
            var dealtRecovery = IBattleLifeStats.DistributeRecovery(BattleStats, incomingHeal);
            BattleStats.PureArmor += dealtRecovery.TotalArmorRecovery;
            BattleStats.Health += dealtRecovery.TotalHealthRecovery;
            Healed?.Invoke(dealtRecovery);
            return dealtRecovery;
        }

        private void OnDeath()
        {
            if (IsAlive) return;//Logging.LogException( new InvalidOperationException("Entity is alive.");
            Died?.Invoke(this);
            DisposeFromBattle();
        }
        #endregion

        #region EntityMover
        public Vector2Int Position => DeployedBattleMap.GetPosition(this);
        public bool CanMove => !StatusHolder.HasStatus(BattleStatus.CantMove) && !IsDisposedFromBattle;

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
            if (value < 0) Logging.LogException(new ArgumentException("Try add action point with less zero value"));
            if (!_actionPoints.ContainsKey(actionPoint)) _actionPoints.Add(actionPoint, 0);
            _actionPoints[actionPoint] += value;
        }
        public void RemoveActionPoints(ActionPoint actionPoint, int value = 1)
        {
            if (value < 0) Logging.LogException(new ArgumentException("Try remove action point with less zero value"));
            if (!_actionPoints.ContainsKey(actionPoint)) Logging.LogException(new ArgumentException("Try remove unavailable actionPoint type"));
            if (_actionPoints[actionPoint] < value) Logging.LogException(new ArgumentOutOfRangeException("Entity doesn't have enough points to be removed.")) ;
            _actionPoints[actionPoint] -= value;
        }
        public void RemoveActionPoints(IReadOnlyDictionary<ActionPoint, int> actionPoints)
        {
            foreach (var point in actionPoints.Keys)
            {
                if (ActionPoints[point] < actionPoints[point])
                    Logging.LogException(new ArgumentOutOfRangeException("Entity doesn't have enough points to be removed.")) ;
            }
            foreach (var point in actionPoints.Keys)
            {
                RemoveActionPoints(point, actionPoints[point]);
            }
        }
        public void SetActionPoints(ActionPoint actionPoint, int value)
        {
            if (value < 0) Logging.LogException( new ArgumentOutOfRangeException());
            if (!_actionPoints.ContainsKey(actionPoint)) 
                _actionPoints.Add(actionPoint, 0);
            _actionPoints[actionPoint] = value;
        }
        private readonly List<ActiveAbilityRunner> _activeAbilities = new();
        private readonly List<PassiveAbilityRunner> _passiveAbilities = new();
        public IReadOnlyList<ActiveAbilityRunner> ActiveAbilities => _activeAbilities;
        public IReadOnlyList<PassiveAbilityRunner> PassiveAbilities => _passiveAbilities;
        public bool IsPerformingAbility { get; set; } //Performs ability
        public void GrantActiveAbility(ActiveAbilityRunner ability)
        {
            _activeAbilities.Add(ability);
        }
        public bool RemoveActiveAbility(ActiveAbilityRunner ability)
        {
            return _activeAbilities.Remove(ability);
        }
        public void GrantPassiveAbility(PassiveAbilityRunner ability)
        {
            _passiveAbilities.Add(ability);
        }
        public bool RemovePassiveAbility(PassiveAbilityRunner ability)
        {
            return _passiveAbilities.Remove(ability);
        }
        #endregion

        #region IEffectHolder
        private readonly HashSet<BattleEffect> _effects = new HashSet<BattleEffect>();

        public IEnumerable<BattleEffect> Effects => _effects;
        public bool HasEffect(IEffectData effect) => _effects.Any(e => e.EffectData == effect);
        public BattleEffect[] GetEffects(IEffectData effectData)
            => _effects.Where(e => e.EffectData == effectData).ToArray();
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
