using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public interface IActionProcessor
    {
        public IEffect[] Effects { get; }
        //public EquipmentItem[] { get; }

        public TAction ProcessOutcomingAction<TAction>(TAction action) where TAction : IBattleAction
        {
            var processedAction = action;
            foreach (var effect in GetOutcomingActionProcessingEffects<TAction>())
            {
                processedAction = effect.ProcessOutcomingAction(processedAction);
                if (processedAction == null)
                    return default;
                // TODO(Critical) Если Action будет структурой, default может работать неправильно
                // Возможные решения: 
                // 1. Делать Action классом
                // 2. Изменить структуру метода на bool TryProcessAction(TAction action, out TAction processedAction)
            }
            return processedAction;
        }

        public TAction ProcessIncomingAction<TAction>(TAction action) where TAction : IBattleAction;

        protected IIncomingActionProcessingEffect<TAction>[] GetIncomingActionProcessingEffects<TAction>()
            where TAction : IBattleAction
        {
            return Effects
                .Select(e => e as IIncomingActionProcessingEffect<TAction>)
                .Where(e => e != null)
                .ToArray();
        }

        protected IOutcomingActionProcessingEffect<TAction>[] GetOutcomingActionProcessingEffects<TAction>()
            where TAction : IBattleAction
        {
            return Effects
                .Select(e => e as IOutcomingActionProcessingEffect<TAction>)
                .Where(e => e != null)
                .ToArray();
        }
    }
}
