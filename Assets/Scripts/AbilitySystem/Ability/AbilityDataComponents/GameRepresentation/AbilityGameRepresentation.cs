namespace OrderElimination.AbilitySystem
{
    public class AbilityGameRepresentation : IAbilityGameRepresentation
    {
        public AbilityType AbilityType { get; private set; }
        public int CooldownTime { get; private set; }

        public TargetingSystemRepresentation TargetingSystem { get; private set; }
        public float? Range { get; private set; }
        public DamageRepresentation DamageRepresentation { get; private set; }

        //AbilityTags[] Tags; //Melee, Range, Damage, ...
        //ActivationType: Manual, Automatic, Combined
        //Cell groups mapping (0 - MainTarget, 1, 3 - Area, ...)

        private AbilityGameRepresentation()
        {

        }

        public static AbilityGameRepresentation FromActiveAbility(
            AbilityRules rules,
            int cooldown,
            IAbilityTargetingSystem targetingSystem,
            ActiveAbilityExecution activeFunctional)
        {
            var representation = new AbilityGameRepresentation();
            var targetingRepresentation = new TargetingSystemRepresentation(targetingSystem);
            representation.AbilityType = AbilityType.Active;
            representation.CooldownTime = cooldown;
            //...
            return representation;
        }

        public static AbilityGameRepresentation FromPassiveAbility(
            int cooldown,
            PassiveAbilityExecution passiveFunctional)
        {
            var representation = new AbilityGameRepresentation();
            representation.AbilityType = AbilityType.Passive;
            representation.CooldownTime = cooldown;
            //...
            return representation;
        }
    }
}
