using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public class ValueCalculationContext
    {
        //private readonly Dictionary<int, float> _contextVariables = new();

        public readonly IBattleContext BattleContext;
        public readonly CellGroupsContainer CellTargetGroups;
        public readonly AbilitySystemActor ActionMaker;
        public readonly AbilitySystemActor ActionTarget;

        public ValueCalculationContext(
            IBattleContext battleContext, 
            CellGroupsContainer cellTargetGroups, 
            AbilitySystemActor actionMaker, 
            AbilitySystemActor actionTarget)
        {
            BattleContext = battleContext;
            CellTargetGroups = cellTargetGroups;
            ActionMaker = actionMaker;
            ActionTarget = actionTarget;
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
