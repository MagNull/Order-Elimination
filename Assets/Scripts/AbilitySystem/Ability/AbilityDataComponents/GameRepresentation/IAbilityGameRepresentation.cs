namespace OrderElimination.AbilitySystem
{
    public interface IAbilityGameRepresentation
    {
        public AbilityType AbilityType { get; }
        public int CooldownTime { get; }
        public TargetingSystemRepresentation TargetingSystem { get; }
        public float? Range { get; }
        public DamageRepresentation DamageRepresentation { get; }
    }
}
