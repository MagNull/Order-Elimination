namespace OrderElimination.AbilitySystem.GameRepresentation
{
    public class DamageRepresentation
    {
        public DamageRepresentation(
            //EntityFilter affectedEntities,
            InflictDamageAction damageAction, 
            int localRepetitions, 
            int totalRepetitions)
        {
            UnprocessedDamageAction = damageAction;
            LocalRepetitions = localRepetitions;
            TotalRepetitions = totalRepetitions;
            //AffectedEntities = affectedEntities.Clone();
        }
        public IContextValueGetter UnprocessedDamageSize => UnprocessedDamageAction?.DamageSize;
        public IContextValueGetter UnprocessedAccuracySize => UnprocessedDamageAction?.Accuracy;
        public InflictDamageAction UnprocessedDamageAction { get; } //safe copy or readonly
        public int LocalRepetitions { get; }
        public int TotalRepetitions { get; } //Considers parent instruction repetitions
        //DamageTarget/TargetGroup
        //public EntityFilter AffectedEntities { get; } = new();
        //Affected cells/cellGroups?

        public InflictDamageAction GetProcessedParameters(
            ActionContext actionContext, out DamageInfo damage, out float accuracy)
        {
            var processedAction = UnprocessedDamageAction.GetModifiedAction(actionContext);
            damage = processedAction.CalculateDamage(actionContext);
            accuracy = processedAction.CalculateAccuracy(actionContext);
            return processedAction;
        }
    }
}
