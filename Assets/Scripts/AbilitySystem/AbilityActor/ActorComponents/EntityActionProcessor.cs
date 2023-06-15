using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.AbilitySystem
{
    //TODO-ОБРАБОТКА объеденить список эффектов и взаимодействие с ними у Процессора и Актора. 
    //Либо вынести методы обработки в Актора.
    public class EntityActionProcessor
    {
        private readonly IEffectHolder _effectHolder;
        //Equipments

        private EntityActionProcessor(IEffectHolder effectHolder)//equipHolder
        {
            _effectHolder = effectHolder;
            //equip
        }

        public static EntityActionProcessor Create<TOwner>(TOwner owner) 
            where TOwner : IEffectHolder//, EquipHolder // Replace with one common interface?
            => new EntityActionProcessor(owner);

        public TAction ProcessIncomingAction<TAction>(TAction action, ActionContext performContext)
            where TAction : BattleAction<TAction>
        {
            return ProcessAction(action, performContext, GetIncomingEffectProcessors());
        }

        public TAction ProcessOutcomingAction<TAction>(TAction action, ActionContext performContext)
            where TAction : BattleAction<TAction>
        {
            return ProcessAction(action, performContext, GetOutcomingEffectProcessors());
        }

        private IEnumerable<IActionProcessor> GetIncomingEffectProcessors()
        {
            return _effectHolder
                .Effects
                .Where(e => e.EffectData.IncomingActionProcessor != null)
                .Select(e => e.EffectData.IncomingActionProcessor);
        }

        private IEnumerable<IActionProcessor> GetOutcomingEffectProcessors()
        {
            return _effectHolder
                .Effects
                .Where(e => e.EffectData.OutcomingActionProcessor != null)
                .Select(e => e.EffectData.OutcomingActionProcessor);
        }

        private TAction ProcessAction<TAction>(
            TAction action, 
            ActionContext performContext, 
            IEnumerable<IActionProcessor> processors)
            where TAction : BattleAction<TAction>
        {
            var processedAction = action;
            foreach (var processor in processors)
            {
                processedAction = processor.ProcessAction(processedAction, performContext);
                if (processedAction == null)
                    Logging.LogException( new ArgumentNullException(nameof(processedAction)));
            }
            return processedAction;
        }
    }
}
