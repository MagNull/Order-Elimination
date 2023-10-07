using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Linq;

namespace OrderElimination.AbilitySystem.Conditions
{
    public class HasEffectCondition : IEntityCondition
    {
        [ShowInInspector, OdinSerialize]
        public IEffectData[] RequiredEffects { get; private set; } = new IEffectData[0];

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
            if (RequiredEffects == null) 
                Logging.LogException(new InvalidOperationException());
            if (entityToCheck == null) 
                Logging.LogException(new ArgumentNullException());
            return EffectRequirement switch
            {
                RequireType.All => RequiredEffects.All(effect => HasRequiredEffect(effect)),
                RequireType.Any => RequiredEffects.Any(effect => HasRequiredEffect(effect)),
                _ => throw new NotImplementedException(),
            };

            bool HasRequiredEffect(IEffectData effectData)
            {
                if (IsAppliedByAskingEntity)
                {
                    var requiredEffects = entityToCheck.GetEffects(effectData);
                    if (requiredEffects.Length == 0)
                        return false;
                    return requiredEffects.Any(e => e.EffectApplier == askingEntity);
                }
                return entityToCheck.HasEffect(effectData);
            }
        }

        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor askingEntity, AbilitySystemActor entityToCheck, CellGroupsContainer cellGroups)
            => IsConditionMet(battleContext, askingEntity, entityToCheck);
    }
}
