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
            EntityType = type;
            BattleSide = side;
            foreach (var p in EnumExtensions.GetValues<EnergyPoint>())
            {
                _energyPoints.Add(p, 0);
            }
            _actionProcessor = new Lazy<EntityActionProcessor>(() => EntityActionProcessor.Create(this));
            _obstacle = new Lazy<BattleObstacle>(() => new BattleObstacle(obstacleSetup, this));
            StatusHolder = new EntityStatusHolder();
            StatusHolder.StatusAppeared += OnStatusAppeared;
            StatusHolder.StatusDisappeared += OnStatusDisappeared;

            void OnStatusAppeared(BattleStatus status)
            {
                if (status == BattleStatus.PassiveAbilitiesDisabled)
                {
                    foreach (var passiveAbility in PassiveAbilities)
                    {
                        passiveAbility.Deactivate();
                    }
                }
            }

            void OnStatusDisappeared(BattleStatus status)
            {
                if (status == BattleStatus.PassiveAbilitiesDisabled)
                {
                    foreach (var passiveAbility in PassiveAbilities)
                    {
                        passiveAbility.Activate(BattleContext, this);
                    }
                }
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
        public event Action<DealtDamageInfo> OnLethalDamage;
        public event Action<AbilitySystemActor> Died;

        public DealtDamageInfo TakeDamage(DamageInfo incomingDamage)
        {
            if (StatusHolder.HasStatus(BattleStatus.Invulnerable))
            {
                return new DealtDamageInfo(incomingDamage, 0, 0);
            }
            var dealtDamage = IBattleLifeStats.DistributeDamage(BattleStats, incomingDamage);
            BattleStats.TotalArmor -= dealtDamage.DealtDamageToArmor;
            BattleStats.Health -= dealtDamage.DealtDamageToHealth;
            if (dealtDamage.DealtDamageToHealth >= BattleStats.Health)
            {
                OnLethalDamage?.Invoke(dealtDamage);
            }
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
        private readonly Dictionary<EnergyPoint, int> _energyPoints = new();
        public IReadOnlyDictionary<EnergyPoint, int> EnergyPoints => _energyPoints;
        public void AddEnergyPoints(EnergyPoint energyPoint, int value = 1)
        {
            if (value < 0)
                Logging.LogException(
                    new ArgumentException("Attempt to remove energy point with less zero value"));
            if (!_energyPoints.ContainsKey(energyPoint))
                _energyPoints.Add(energyPoint, 0);
            _energyPoints[energyPoint] += value;
        }
        public bool RemoveEnergyPoints(EnergyPoint energyPoint, int value = 1)
        {
            if (value < 0) 
                Logging.LogException(
                    new ArgumentException("Attempt to remove energy point with less zero value"));
            if (!_energyPoints.ContainsKey(energyPoint)) 
                Logging.LogException(new ArgumentException("Try remove unavailable actionPoint type"));
            if (value <= _energyPoints[energyPoint])
            {
                _energyPoints[energyPoint] -= value;
                return true;
            }
            _energyPoints[energyPoint] -= Mathf.Min(_energyPoints[energyPoint], value);
            return false;
        }
        public void RemoveEnergyPoints(IReadOnlyDictionary<EnergyPoint, int> energyPoint)
        {
            foreach (var point in energyPoint.Keys)
            {
                if (EnergyPoints[point] < energyPoint[point])
                    Logging.LogException(new ArgumentOutOfRangeException("Entity doesn't have enough points to be removed.")) ;
            }
            foreach (var point in energyPoint.Keys)
            {
                RemoveEnergyPoints(point, energyPoint[point]);
            }
        }
        public void SetEnergyPoints(EnergyPoint energyPoint, int value)
        {
            if (value < 0)
                Logging.LogException(new ArgumentOutOfRangeException());
            if (!_energyPoints.ContainsKey(energyPoint)) 
                _energyPoints.Add(energyPoint, 0);
            _energyPoints[energyPoint] = value;
        }
        public void ClearEnergyPoints(EnergyPoint energyPoint)
        {
            if (_energyPoints.ContainsKey(energyPoint))
                _energyPoints[energyPoint] = 0;
        }
        //event EnergyPoint EnergyPointsChanged

        private readonly List<ActiveAbilityRunner> _activeAbilities = new();
        private readonly List<PassiveAbilityRunner> _passiveAbilities = new();
        public IReadOnlyList<ActiveAbilityRunner> ActiveAbilities => _activeAbilities;
        public IReadOnlyList<PassiveAbilityRunner> PassiveAbilities => _passiveAbilities;
        public bool IsPerformingAbility { get; set; } //Performs ability

        public event Action<AbilitySystemActor> AbilitiesChanged;

        public void GrantActiveAbility(ActiveAbilityRunner ability)
        {
            _activeAbilities.Add(ability);
            AbilitiesChanged?.Invoke(this);
        }
        public bool RemoveActiveAbility(ActiveAbilityRunner ability)
        {
            var result = _activeAbilities.Remove(ability);
            AbilitiesChanged?.Invoke(this);
            return result;
        }
        public void GrantPassiveAbility(PassiveAbilityRunner ability)
        {
            _passiveAbilities.Add(ability);
            AbilitiesChanged?.Invoke(this);
        }
        public bool RemovePassiveAbility(PassiveAbilityRunner ability)
        {
            var result = _passiveAbilities.Remove(ability);
            AbilitiesChanged?.Invoke(this);
            return result;
        }
        #endregion

        #region IEffectHolder
        private readonly HashSet<BattleEffect> _effects = new();
        private readonly List<IEffectData> _effectImmunities = new();

        public IEnumerable<BattleEffect> Effects => _effects;
        public IReadOnlyList<IEffectData> EffectImmunities => _effectImmunities;

        public event Action<BattleEffect> EffectAdded;
        public event Action<BattleEffect> EffectRemoved;
        public event Action<IEffectData> EffectBlockedByImmunity;

        public bool HasEffect(IEffectData effect) => _effects.Any(e => e.EffectData == effect);

        public BattleEffect[] GetEffects(IEffectData effectData)
            => _effects.Where(e => e.EffectData == effectData).ToArray();

        public bool ApplyEffect(IEffectData effectData, AbilitySystemActor applier, out BattleEffect appliedEffect)
        {
            var effect = new BattleEffect(effectData, BattleContext);
            var applyResult = effectData.CanBeAppliedOn(this);
            if (applyResult == EffectApplyResult.Success)
            {
                if (!effect.Apply(this, applier))
                    Logging.LogError("Successful effect apply was promised but apply failed.");
                _effects.Add(effect);
                effect.Deactivated += OnEffectDeactivated;
                EffectAdded?.Invoke(effect);
                effect.Activate();
                appliedEffect = effect;
                return true;
            }
            if (applyResult == EffectApplyResult.BlockedByImmunity)
            {
                EffectBlockedByImmunity?.Invoke(effectData);
            }
            appliedEffect = null;
            return false;
        }

        public bool RemoveEffect(BattleEffect effect)
        {
            return _effects.Contains(effect) && effect.TryDeactivate();
        }

        public void AddEffectImmunity(IEffectData effect)
        {
            _effectImmunities.Add(effect);
        }

        public bool RemoveEffectImmunity(IEffectData effect)
        {
            return _effectImmunities.Remove(effect);
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

        public EntityStatusHolder StatusHolder { get; }

        public EntityActionProcessor ActionProcessor => _actionProcessor.Value;

        public BattleObstacle Obstacle => _obstacle.Value;
    }
}
