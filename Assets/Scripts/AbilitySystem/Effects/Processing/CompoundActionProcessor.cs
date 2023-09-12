using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;

namespace OrderElimination.AbilitySystem
{
    public class CompoundActionProcessor : IActionProcessor
    {
        [ShowInInspector, OdinSerialize]
        private List<IActionProcessor> _processors { get; set; } = new();

        public IReadOnlyList<IActionProcessor> Processors => _processors;

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
}
