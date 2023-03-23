using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    //К IBattleEntity относятся здания и персонажи
    public interface IBattleEntity : IActionTarget
    {
        public bool TakeDamage(DamageInfo incomingDamage, out DamageInfo takenDamage);
        public bool ApplyEffect(IEffect effect); //нестакающиеся эффекты не должны добавляться
    }

    public interface IAbilitySystemActor // Интерфейс взаимодействия со способностями (замена IBattleEntity)
    {
        //public IStatAttributeSets: HealthArmorAttributes
        //или
        //IBattleStats(CharacterStats, BuildingStats, ...)

        #region IEffectHolder
        public IEffect[] Effects { get; }
        public event Action<IEffect> EffectAdded;
        public event Action<IEffect> EffectRemoved;
        public bool CanApplyEffect(IEffect effect);
        public bool ApplyEffect(IEffect effect); //if effect exist on actor and not stackable...
        public bool RemoveEffect(IEffect effect);
        #endregion

        public Ability[] PossessedAbilities { get; }
        //public bool UseAbility(Ability ability, CellTargetGroups targets); //TODO return AbilityUseContext
        public bool MakeAction(IBattleAction action, IAbilitySystemActor actionTarget); //TODO return ActionUseContext
        public bool RecieveAction(IBattleAction action, IAbilitySystemActor actionMaker); //TODO return ActionUseContext

        #region IDamagable
        public bool TakeDamage(DamageInfo incomingDamage, out DamageInfo takenDamage);//hit? miss? evasion? takenDamage?
        #endregion
        #region IHealable
        public bool TakeHeal(HealInfo incomingHeal, out HealInfo takenHeal);
        #endregion
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
