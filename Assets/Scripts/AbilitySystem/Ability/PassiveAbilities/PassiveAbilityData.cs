namespace OrderElimination.AbilitySystem
{
    public class PassiveAbilityData : IPassiveAbilityData
    {
        public AbilityView View { get; set; }
        public AbilityGameRepresentation GameRepresentation { get; set; }
        //public Triggers[] //For Passive Abilities ??
        //AutomatedDistributionPattern //For Passive Abilities
        public PassiveAbilityExecution Execution { get; set; }
        
        public PassiveAbilityBuilder BasedBuilder { get; set; }
    }

    public interface IPassiveAbilityData
    {
        public AbilityView View { get; }
        public AbilityGameRepresentation GameRepresentation { get; }
        public PassiveAbilityExecution Execution { get; }
        
        public PassiveAbilityBuilder BasedBuilder { get; }
    }
}
