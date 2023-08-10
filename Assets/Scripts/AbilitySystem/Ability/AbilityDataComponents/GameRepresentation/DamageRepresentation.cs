namespace OrderElimination.AbilitySystem
{
    public class DamageRepresentation
    {
        public DamageRepresentation(
            EntityFilter affectedEntities,
            InflictDamageAction damageAction, 
            int localRepetitions, 
            int totalRepetitions)
        {
            DamageAction = damageAction;
            LocalRepetitions = localRepetitions;
            TotalRepetitions = totalRepetitions;
            TargetFilter = affectedEntities.Clone();
        }

        public InflictDamageAction DamageAction { get; } //safe copy or readonly
        public int LocalRepetitions { get; }
        public int TotalRepetitions { get; } //Considers higher instruction repetitions
        //DamageTarget/TargetGroup
        public EntityFilter TargetFilter { get; } = new();
        //Affected cells/cellGroups?
        //GetProcessedDamage(ActionContext)
        //DamageSize
        //Accuracy
        public InflictDamageAction GetContextDamage(
            ActionContext actionContext, out DamageInfo damage, out float accuracy)
        {
            var processedAction = DamageAction.GetModifiedAction(actionContext);
            damage = processedAction.CalculateDamage(actionContext);
            accuracy = processedAction.CalculateAccuracy(actionContext);
            return processedAction;
        }

        //public bool GetContextIndependentDamage(out DamageInfo damage, out float accuracy)
        //{

        //}
    }
}
