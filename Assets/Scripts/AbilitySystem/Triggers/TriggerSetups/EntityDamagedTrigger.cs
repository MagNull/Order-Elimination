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
            instance.Activated += OnActivation;
            return instance;

            void OnActivation(ITriggerSetup.BattleTrigger trigger)
            {
                instance.Activated -= OnActivation;
                instance.Deactivated += OnDeactivation;
                trackingEntity.Damaged += OnDamaged;

            }

            void OnDeactivation(ITriggerSetup.BattleTrigger trigger)
            {
                instance.Deactivated -= OnDeactivation;
                trackingEntity.Damaged -= OnDamaged;
            }

            void OnDamaged(DealtDamageInfo damageInfo)
            {
                instance.Trigger(new EntityDamagedTriggerFireInfo(damageInfo));
            }
        }
    }

    public readonly struct EntityDamagedTriggerFireInfo : ITriggerFireInfo
    {
        public readonly DealtDamageInfo DamageInfo;

        public EntityDamagedTriggerFireInfo(DealtDamageInfo damageInfo)
        {
            DamageInfo = damageInfo;
        }
    }
}
