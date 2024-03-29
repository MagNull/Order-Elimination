﻿using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class EntityDamagedTrigger : IEntityTriggerSetup
    {
        [ShowInInspector, OdinSerialize]
        public EnumMask<DamageType> TriggeringDamageTypes { get; private set; } = EnumMask<DamageType>.Full;

        [ValidateInput("@false", "*Only Target entity (Tracking entity) is available.")]
        [ShowInInspector, OdinSerialize]
        public IContextValueGetter MinDamageThreshold { get; private set; } = new ConstValueGetter(0);

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
                var calculationContext = ValueCalculationContext.Full(
                    battleContext,
                    CellGroupsContainer.Empty,
                    null,//Require trigger activator-entity?
                    trackingEntity);
                if (TriggeringDamageTypes[damageInfo.IncomingDamage.DamageType]
                    && damageInfo.TotalDealtDamage >= MinDamageThreshold.GetValue(calculationContext))
                {
                    instance.FireTrigger(new EntityDamagedTriggerFireInfo(instance, damageInfo));
                }
            }
        }
    }
}
