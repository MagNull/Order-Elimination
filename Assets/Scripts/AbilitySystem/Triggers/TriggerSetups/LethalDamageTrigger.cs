using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class LethalDamageTrigger : IEntityTriggerSetup
    {
        [ShowInInspector, OdinSerialize]
        public EnumMask<DamageType> TriggeringDamageTypes { get; private set; } = EnumMask<DamageType>.Full;

        public IBattleTrigger GetTrigger(
            IBattleContext battleContext, AbilitySystemActor trackingEntity)
        {
            var instance = new ITriggerSetup.BattleTrigger(this, battleContext);
            instance.ActivationRequested += OnActivation;
            return instance;

            void OnActivation(ITriggerSetup.BattleTrigger trigger)
            {
                instance.ActivationRequested -= OnActivation;
                instance.DeactivationRequested += OnDeactivation;
                trackingEntity.OnLethalDamage += OnLethalDamage;

            }

            void OnDeactivation(ITriggerSetup.BattleTrigger trigger)
            {
                instance.DeactivationRequested -= OnDeactivation;
                trackingEntity.OnLethalDamage -= OnLethalDamage;
            }

            void OnLethalDamage(DealtDamageInfo damageInfo)
            {
                if (TriggeringDamageTypes[damageInfo.IncomingDamage.DamageType])
                {
                    instance.FireTrigger(new EntityDamagedTriggerFireInfo(instance, damageInfo));
                }
            }
        }
    }
}
