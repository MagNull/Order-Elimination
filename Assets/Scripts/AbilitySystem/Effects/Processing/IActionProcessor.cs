using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.UIElements;

namespace OrderElimination.AbilitySystem
{
    public interface IActionProcessor
    {
        public TAction ProcessAction<TAction>(TAction originalAction, ActionContext performContext)
            where TAction : BattleAction<TAction>;
    }

    public class CompoundActionProcessor : IActionProcessor
    {
        [ShowInInspector, OdinSerialize]
        private List<IActionProcessor> _processors { get; set; } = new();

        public TAction ProcessAction<TAction>(TAction originalAction, ActionContext performContext)
            where TAction : BattleAction<TAction>
        {
            var modifiedAction = originalAction;
            foreach (var processor in _processors)
            {
                modifiedAction = processor.ProcessAction(modifiedAction, performContext);
            }
            return modifiedAction;
        }
    }

    public class ConditionalActionProcessor : IActionProcessor
    {
        [ShowInInspector, OdinSerialize]
        private List<IEntityCondition> _casterConditions { get; set; } = new();

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
            if (_casterConditions.Count > 0 && caster == null)
                return originalAction;
            if (_targetConditions.Count > 0 && target == null)
                return originalAction;
            var casterConditions = _casterConditions.All(c => c.IsConditionMet(battleContext, caster, caster));
            var targetConditions = _targetConditions.All(c => c.IsConditionMet(battleContext, caster, target));
            if (casterConditions && targetConditions)
                return _actionProcessor.ProcessAction(originalAction, performContext);
            return originalAction;
        }
    }
}
