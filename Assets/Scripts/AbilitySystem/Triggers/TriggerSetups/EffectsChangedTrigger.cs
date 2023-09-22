using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
namespace OrderElimination.AbilitySystem
{
    public class EffectsChangedTrigger : IEntityTriggerSetup
    {
        [ShowInInspector, OdinSerialize]
        private HashSet<IEffectData> _trackingEffects = new();

        [ShowInInspector, OdinSerialize]
        private bool TriggerOnApply { get; set; }

        [ShowInInspector, OdinSerialize]
        private bool TriggerOnRemove { get; set; }

        //bool IgnoreSimultaneousChanges ? //might be hard to implement

        public IBattleTrigger GetTrigger(IBattleContext battleContext, AbilitySystemActor trackingEntity)
        {
            var instance = new ITriggerSetup.BattleTrigger(this, battleContext);
            instance.ActivationRequested += OnActivation;
            return instance;

            void OnActivation(ITriggerSetup.BattleTrigger trigger)
            {
                instance.ActivationRequested -= OnActivation;
                instance.DeactivationRequested += OnDeactivation;
                if (_trackingEffects.Count == 0)
                    throw new InvalidOperationException("No effects to track");
                if (TriggerOnApply)
                    trackingEntity.EffectAdded += OnEffectAdded;
                if (TriggerOnRemove)
                    trackingEntity.EffectRemoved += OnEffectRemoved;
            }

            void OnDeactivation(ITriggerSetup.BattleTrigger trigger)
            {
                instance.DeactivationRequested -= OnDeactivation;
                trackingEntity.EffectAdded -= OnEffectAdded;
                trackingEntity.EffectRemoved -= OnEffectRemoved;
            }

            void OnEffectAdded(BattleEffect effect)
            {
                if (_trackingEffects.Contains(effect.EffectData))
                {
                    instance.FireTrigger(new EffectsChangedFireInfo(
                        instance, effect, EffectsChangedFireInfo.EffectChangeType.EffectAdded));
                }
            }

            void OnEffectRemoved(BattleEffect effect)
            {
                if (_trackingEffects.Contains(effect.EffectData))
                {
                    instance.FireTrigger(new EffectsChangedFireInfo(
                        instance, effect, EffectsChangedFireInfo.EffectChangeType.EffectRemoved));
                }
            }
        }
    }
}
