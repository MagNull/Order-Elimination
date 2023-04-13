using System;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace OrderElimination.AbilitySystem
{
    public abstract class IAbilitySystemActor : IHaveLifeStats, IEffectHolder
    {
        #region IHaveLifeStats
        public ILifeStats LifeStats => BattleStats;
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
        public ActorType ActorType { get; }
        //Side (player, enemy, neutral, ...)

        public IBattleStats BattleStats { get; }

        public IReadOnlyDictionary<ActionPoint, int> ActionPoints => _actionPoints;
        public void AddActionPoint(ActionPoint actionPoint, int value = 1)
        {
            if (!_actionPoints.ContainsKey(actionPoint))
                _actionPoints.Add(actionPoint, 0);
            _actionPoints[actionPoint] += value;

        }
        public void RemoveActionPoint(ActionPoint actionPoint, int value = 1)
        {
            if (!_actionPoints.ContainsKey(actionPoint))
                _actionPoints.Add(actionPoint, 0);
            if (_actionPoints[actionPoint] < value) throw new ArgumentOutOfRangeException();
            _actionPoints[actionPoint] -= value;

        }

        public ActionProcessor ActionProcessor { get; }

        #region IEffectHolder
        public IEnumerable<IEffect> Effects => _effects;
        public bool HasEffect(IEffect effect) => _effects.Contains(effect);
        public bool CanApplyEffect(IEffect effect) => !HasEffect(effect) || effect.EffectData.IsStackable;
        public event Action<IEffect> EffectAdded;
        public event Action<IEffect> EffectRemoved;

        public bool ApplyEffect(IEffect effect)
        {
            if (CanApplyEffect(effect) && effect.Activate(this))
            {
                _effects.Add(effect);
                return true;
            }
            return false;
        }

        public bool RemoveEffect(IEffect effect)
        {
            if (effect.EffectData.CanBeForceRemoved)
                return _effects.Remove(effect);
            return false;
        }
        #endregion

        //public Ability[] PossessedAbilities { get; }
        //public bool UseAbility(Ability ability, CellTargetGroups targets); //TODO return AbilityUseContext


        //IBattleObstacle?

        private readonly HashSet<IEffect> _effects = new HashSet<IEffect>();
        private readonly Dictionary<ActionPoint, int> _actionPoints = new Dictionary<ActionPoint, int>();
    }

    public interface IAbilitySystemActorView
    {
        public IAbilitySystemActor Model { get; }
        //public DamageInfo ShowModifiedDamage(DamageInfo incomingDamage);
        //public float ShowModifiedAccuracy(float incomingAccuracy);
        //public HealInfo ShowModifiedHeal(HealInfo incomingHeal);
        //public TEffect ShowModifiedApplyingEffect<TEffect>(TEffect effect) where TEffect : IEffect;
    }

    public enum ActorType
    {
        Unit,
        MapObject
    }
}
