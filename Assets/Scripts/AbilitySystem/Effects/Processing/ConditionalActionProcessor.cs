using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.AbilitySystem
{
    public class ConditionalActionProcessor : IActionProcessor
    {
        [ShowInInspector, OdinSerialize]
        private List<ICommonCondition> _commonConditions { get; set; } = new();

        [ShowInInspector, OdinSerialize]
        private List<IEntityCondition> _targetConditions { get; set; } = new();

        [ShowInInspector, OdinSerialize]
        private IActionProcessor _actionProcessor { get; set; }

        public TAction ProcessAction<TAction>(TAction originalAction, ActionContext performContext)
            where TAction : BattleAction<TAction>
        {
            var battleContext = performContext.BattleContext;
            var caster = performContext.ActionMaker;
            var target = performContext.ActionTarget;
            if (_commonConditions.Count > 0 && caster == null)
                return originalAction;
            if (_targetConditions.Count > 0 && target == null)
                return originalAction;
            if (!_commonConditions.All(c => c.IsConditionMet(battleContext, caster)))
                return originalAction;
            if (!_targetConditions.All(c => c.IsConditionMet(battleContext, caster, target)))
                return originalAction;
            return _actionProcessor.ProcessAction(originalAction, performContext);
        }
    }
}
