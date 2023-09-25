using OrderElimination.Infrastructure;
using System.Collections.Generic;

namespace OrderElimination.AbilitySystem.GameRepresentation
{
    public interface IAbilityGameRepresentation
    {
        public AbilityType AbilityType { get; }
        public int CooldownTime { get; }
        public TargetingSystemRepresentation TargetingSystem { get; }
        //public float? MaxRange { get; }
        public IReadOnlyList<DamageRepresentation> DamageRepresentations { get; }
        public IReadOnlyList<HealRepresentation> HealRepresentations { get; }
        //public EnumMask<AbilityRole> Attack/Heal/Move/Other
        public IReadOnlyList<ApplyEffectRepresentation> EffectRepresentations { get; }
    }
}
