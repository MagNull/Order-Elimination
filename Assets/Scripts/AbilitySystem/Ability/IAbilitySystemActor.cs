using System;
using System.Collections.Generic;

namespace OrderElimination.AbilitySystem
{
    public interface IAbilitySystemActor // Интерфейс взаимодействия со способностями (замена IBattleEntity)
    {
        public IBattleStats BattleStats { get; }

        //Side (player, enemy, neutral, ...)

        public IReadOnlyDictionary<ActionPoint, int> ActionPoints { get; }
        public void AddActionPoint(ActionPoint actionPoint, int value = 1);
        public bool RemoveActionPoint(ActionPoint actionPoint, int value = 1);
        //public Ability[] PossessedAbilities { get; }
        //public bool UseAbility(Ability ability, CellTargetGroups targets); //TODO return AbilityUseContext

        public IActionProcessor ActionProcessor { get; }
        //TODO-ОБРАБОТКА Эффекты сейчас внутри Актора и внутри Процессора. Исправить.
        #region IEffectHolder
        public IEffect[] Effects { get; }
        public event Action<IEffect> EffectAdded;
        public event Action<IEffect> EffectRemoved;
        public bool CanApplyEffect(IEffect effect);
        public bool ApplyEffect(IEffect effect); //if effect exist on actor and not stackable...
        public bool RemoveEffect(IEffect effect);
        #endregion

        #region IDamagable
        //Пока что механика уклонения находится внутри InflictDamageAction
        //При необходимости её можно вынести в TakeDamage(), указав в параметрах метода, нужно ли учитывать уклонение.
        //out DamageInfo takenDamage для случаев, когда наносимый урон не соответствует полученному (н: хп персонажа < урона)
        public bool TakeDamage(DamageInfo incomingDamage, out DamageInfo takenDamage);
        //event/Trigger OnTakeDamage
        //event/Trigger OnDead
        #endregion

        #region IHealable
        public bool TakeHeal(HealInfo incomingHeal, out HealInfo takenHeal);
        #endregion

        //IBattleObstacle?
    }

    public interface IAbilitySystemActorView
    {
        public IAbilitySystemActor Model { get; }
        public DamageInfo ShowModifiedDamage(DamageInfo incomingDamage);
        public float ShowModifiedAccuracy(float incomingAccuracy);
        public HealInfo ShowModifiedHeal(HealInfo incomingHeal);
        public TEffect ShowModifiedApplyingEffect<TEffect>(TEffect effect) where TEffect : IEffect;
    }
}
