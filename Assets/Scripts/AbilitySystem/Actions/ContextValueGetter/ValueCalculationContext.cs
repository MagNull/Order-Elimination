namespace OrderElimination.AbilitySystem
{
    public class ValueCalculationContext
    {
        //private readonly Dictionary<int, float> _contextVariables = new();

        public readonly IBattleContext BattleContext;
        public readonly CellGroupsContainer CellTargetGroups;
        public readonly AbilitySystemActor Caster;
        public readonly AbilitySystemActor Target;

        public ValueCalculationContext(
            IBattleContext battleContext, 
            CellGroupsContainer cellTargetGroups, 
            AbilitySystemActor actionMaker, 
            AbilitySystemActor actionTarget)
        {
            BattleContext = battleContext;
            CellTargetGroups = cellTargetGroups;
            Caster = actionMaker;
            Target = actionTarget;
        }

        public static ValueCalculationContext FromActionContext(ActionContext actionContext)
            => new(
                actionContext.BattleContext,
                actionContext.CellTargetGroups,
                actionContext.ActionMaker,
                actionContext.ActionTarget);

        //public IReadOnlyDictionary<int, float> ContextVariables => _contextVariables;

        //public void WriteVariable(int id, float value)
        //{
        //    if (_contextVariables.ContainsKey(id))
        //        throw new InvalidOperationException("Variable with the same name has already been written.");
        //    _contextVariables.Add(id, value);
        //}
    }
}
