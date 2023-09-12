using OrderElimination.Infrastructure;
using System.Collections.Generic;

namespace OrderElimination.AbilitySystem
{
    public interface IAbilityGameRepresentation
    {
        public AbilityType AbilityType { get; }
        public int CooldownTime { get; }
        public TargetingSystemRepresentation TargetingSystem { get; }
        public float? MaxRange { get; }
        public IReadOnlyList<DamageRepresentation> DamageRepresentations { get; }
        //public EnumMask<AbilityRole> Attack/Heal/Move/Other
        public IReadOnlyList<AbilityEffectRepresentation> EffectRepresentations { get; }
    }
}
