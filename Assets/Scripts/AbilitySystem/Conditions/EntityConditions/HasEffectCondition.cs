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

        public IEntityCondition Clone()
        {
            var clone = new HasEffectCondition();
            clone.RequiredEffects = RequiredEffects.ToArray();
            clone.EffectRequirement = EffectRequirement;
            return clone;
        }

        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor askingEntity, AbilitySystemActor entityToCheck)
        {
            if (RequiredEffects == null) Logging.LogException( new InvalidOperationException());
            if (entityToCheck == null) Logging.LogException( new ArgumentNullException());
            return EffectRequirement switch
            {
                RequireType.All => RequiredEffects.All(effect => entityToCheck.HasEffect(effect)),
                RequireType.Any => RequiredEffects.Any(effect => entityToCheck.HasEffect(effect)),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
