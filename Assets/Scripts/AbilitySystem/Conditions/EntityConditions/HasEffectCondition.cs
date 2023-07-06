using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem.Conditions
{
    public class HasEffectCondition : IEntityCondition
    {
        public enum RequireType //TODO: Extract
        {
            All,
            Any
        }

        [ShowInInspector, OdinSerialize]
        public IEffectData[] RequiredEffects { get; private set; }

        [ShowInInspector, OdinSerialize]
        public RequireType EffectRequirement { get; private set; }

        [ShowInInspector, OdinSerialize]
        public bool IsAppliedByAskingEntity { get; private set; }//change to entity conditions

        public IEntityCondition Clone()
        {
            var clone = new HasEffectCondition();
            clone.RequiredEffects = RequiredEffects.ToArray();
            clone.EffectRequirement = EffectRequirement;
            clone.IsAppliedByAskingEntity = IsAppliedByAskingEntity;
            return clone;
        }

        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor askingEntity, AbilitySystemActor entityToCheck)
        {
            if (RequiredEffects == null) Logging.LogException(new InvalidOperationException());
            if (entityToCheck == null) Logging.LogException(new ArgumentNullException());
            return EffectRequirement switch
            {
                RequireType.All => RequiredEffects.All(effect => HasRequiredEffect(entityToCheck, effect)),
                RequireType.Any => RequiredEffects.Any(effect => HasRequiredEffect(entityToCheck, effect)),
                _ => throw new NotImplementedException(),
            };

            bool HasRequiredEffect(AbilitySystemActor entity, IEffectData effectData)
            {
                if (IsAppliedByAskingEntity)
                {
                    return entity.GetEffects(effectData).Any(e => e.EffectApplier == entity);
                }
                return entity.HasEffect(effectData);
            }
        }
    }
}
