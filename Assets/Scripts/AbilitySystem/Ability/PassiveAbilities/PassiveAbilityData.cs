namespace OrderElimination.AbilitySystem
{
    public class PassiveAbilityData : IPassiveAbilityData
    {
        public PassiveAbilityBuilder BasedBuilder { get; set; }
        public PassiveAbilityView View { get; set; }
        public IAbilityGameRepresentation GameRepresentation { get; set; }
        //public Triggers[] //For Passive Abilities ??
        //AutomatedDistributionPattern //For Passive Abilities
        public PassiveAbilityExecution Execution { get; set; }
    }

    public interface IPassiveAbilityData
    {
        public PassiveAbilityBuilder BasedBuilder { get; }
        public PassiveAbilityView View { get; }
        public IAbilityGameRepresentation GameRepresentation { get; }
        public PassiveAbilityExecution Execution { get; }
    }
}
