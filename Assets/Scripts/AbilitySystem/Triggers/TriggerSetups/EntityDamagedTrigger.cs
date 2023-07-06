using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class EntityDamagedTrigger : IEntityTriggerSetup
    {
        [HideInInspector, OdinSerialize]
        private float _minDamageThreshold = 0;

        [ShowInInspector, OdinSerialize]
        public EnumMask<DamageType> TriggeringDamageTypes { get; private set; } = EnumMask<DamageType>.Full;

        [ShowInInspector]
        public float MinDamageThreshold
        {
            get => _minDamageThreshold;
            private set
            {
                if (value < 0) value = 0;
                _minDamageThreshold = value;
            }
        }

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
                if (TriggeringDamageTypes[damageInfo.DamageInfo.DamageType]
                    && damageInfo.TotalDamage >= MinDamageThreshold)
                {
                    instance.FireTrigger(new EntityDamagedTriggerFireInfo(instance, damageInfo));
                }
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
