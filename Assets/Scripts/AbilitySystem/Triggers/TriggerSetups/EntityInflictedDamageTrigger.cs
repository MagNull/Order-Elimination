using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace OrderElimination.AbilitySystem
{
    public class EntityInflictedDamageTrigger : IEntityTriggerSetup, ITriggerWithFireInfo<DamageTriggerFireInfo>
    {
        [ShowInInspector, OdinSerialize]
        public EnumMask<DamageType> TriggeringDamageTypes { get; private set; } = EnumMask<DamageType>.Full;

        [ValidateInput("@false", "*Caster is entity that activated trigger.\n"
            + "Usually it's passive ability's owner or effect applier")]
        [ShowInInspector, OdinSerialize]
        public IContextValueGetter MinDamageThreshold { get; private set; } = new ConstValueGetter(0);

        //TriggerOnDamageTo: Any, Armor, Health
        [ToggleLeft]
        [ShowInInspector, OdinSerialize]
        public bool TriggerOnDamageFromEffects { get; private set; }

        public IBattleTrigger GetTrigger(
            IBattleContext battleContext, AbilitySystemActor activator, AbilitySystemActor trackingEntity)
        {
            var instance = new ITriggerSetup.BattleTrigger(this, battleContext, activator);
            instance.ActivationRequested += OnActivation;
            return instance;

            void OnActivation(ITriggerSetup.BattleTrigger trigger)
            {
                instance.ActivationRequested -= OnActivation;
                instance.DeactivationRequested += OnDeactivation;
                trackingEntity.InflictedDamage += OnDamageInflict;
            }

            void OnDeactivation(ITriggerSetup.BattleTrigger trigger)
            {
                instance.DeactivationRequested -= OnDeactivation;
                trackingEntity.InflictedDamage -= OnDamageInflict;
            }

            void OnDamageInflict(DealtDamageInfo damageInfo)
            {
                var calculationContext = ValueCalculationContext.Full(
                    battleContext,
                    CellGroupsContainer.Empty,
                    activator,
                    trackingEntity);
                if (TriggeringDamageTypes[damageInfo.IncomingDamage.DamageType]
                    && (!damageInfo.IncomingDamage.IsFromEffect || TriggerOnDamageFromEffects)
                    && damageInfo.TotalDealtDamage >= MinDamageThreshold.GetValue(calculationContext))
                {
                    instance.FireTrigger(new DamageTriggerFireInfo(instance, damageInfo));
                }
            }
        }
    }
}
