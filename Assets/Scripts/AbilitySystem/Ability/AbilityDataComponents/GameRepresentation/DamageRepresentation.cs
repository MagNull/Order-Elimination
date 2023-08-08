namespace OrderElimination.AbilitySystem
{
    public class DamageRepresentation
    {
        public DamageRepresentation(
            InflictDamageAction damageAction, int localRepetitions, int totalRepetitions)
        {
            DamageAction = damageAction;
            LocalRepetitions = localRepetitions;
            TotalRepetitions = totalRepetitions;
        }

        public InflictDamageAction DamageAction { get; } //safe copy or readonly
        public int LocalRepetitions { get; }
        public int TotalRepetitions { get; } //Considers higher instruction repetitions
        //DamageTarget/TargetGroup
        public EntityFilter TargetFilter { get; } = new();
        //Affected cells/cellGroups?
        //GetProcessedDamage(ActionContext)
    }

    public class AbilityInstructionDamageRepresentation : DamageRepresentation
    {
        public AbilityInstructionDamageRepresentation(
            InflictDamageAction damageAction, 
            int localRepetitions, 
            int totalRepetitions,
            AbilityInstruction damageInstruction) 
            : base(damageAction, localRepetitions, totalRepetitions)
        {
            if (damageInstruction.Action is not InflictDamageAction)
                throw new System.ArgumentException("Instruction doesn't represent damage action.");
            DamageInstruction = damageInstruction;
        }

        public AbilityInstruction DamageInstruction { get; }
    }

    //TriggerInstructionDamageRepresentation
    //EffectInstruction ...
}
