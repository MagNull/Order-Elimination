using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public class EntityDamagedTrigger : IEntityTriggerSetup
    {
        public IBattleTrigger GetTrigger(IBattleContext battleContext, AbilitySystemActor trackingEntity)
        {
            var instance = new ITriggerSetup.BattleTrigger(this, battleContext);
            instance.ActivationRequested += OnActivation;
            return instance;

            void OnActivation(ITriggerSetup.BattleTrigger trigger)
            {
                instance.ActivationRequested -= OnActivation;
                instance.DeactivationRequested += OnDeactivation;
                trackingEntity.Damaged += OnDamaged;

            }

            void OnDeactivation(ITriggerSetup.BattleTrigger trigger)
            {
                instance.DeactivationRequested -= OnDeactivation;
                trackingEntity.Damaged -= OnDamaged;
            }

            void OnDamaged(DealtDamageInfo damageInfo)
            {
                instance.Trigger(new EntityDamagedTriggerFireInfo(instance, damageInfo));
            }
        }
    }

    public class EntityDamagedTriggerFireInfo : ITriggerFireInfo
    {
        public IBattleTrigger Trigger { get; }
        public DealtDamageInfo DamageInfo { get; }

        public EntityDamagedTriggerFireInfo(IBattleTrigger trigger, DealtDamageInfo damageInfo)
        {
            Trigger = trigger;
            DamageInfo = damageInfo;
        }

    }
}
