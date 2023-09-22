namespace OrderElimination.AbilitySystem.GameRepresentation
{
    public class HealRepresentation
    {
        public HealRepresentation(
            //EntityFilter affectedEntities,
            HealAction healAction,
            int localRepetitions,
            int totalRepetitions)
        {
            UnprocessedHealAction = healAction;
            LocalRepetitions = localRepetitions;
            TotalRepetitions = totalRepetitions;
            //AffectedEntities = affectedEntities.Clone();
        }
        public IContextValueGetter UnprocessedHealSize => UnprocessedHealAction?.HealSize;
        public HealAction UnprocessedHealAction { get; } //safe copy or readonly
        public int LocalRepetitions { get; }
        public int TotalRepetitions { get; } //Considers parent instruction repetitions
        //DamageTarget/TargetGroup
        //public EntityFilter AffectedEntities { get; } = new();
        //Affected cells/cellGroups?

        public HealAction GetProcessedParameters(
            ActionContext actionContext, out RecoveryInfo heal)
        {
            var processedAction = UnprocessedHealAction.GetModifiedAction(actionContext);
            heal = processedAction.CalculateRecovery(actionContext);
            return processedAction;
        }
    }
}
