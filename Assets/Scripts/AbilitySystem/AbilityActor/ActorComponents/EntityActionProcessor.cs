using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Timeline.Actions;

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

        public static EntityActionProcessor Create<TOwner>(TOwner owner) where TOwner : IEffectHolder
            => new EntityActionProcessor(owner);

        public TAction ProcessOutcomingAction<TAction>(TAction action) where TAction : BattleAction<TAction>
        {
            return ProcessAction(action, GetOutcomingEffectProcessors());
        }

        public TAction ProcessIncomingAction<TAction>(TAction action) where TAction : BattleAction<TAction>
        {
            return ProcessAction(action, GetIncomingEffectProcessors());
        }

        private IEnumerable<IActionProcessor> GetOutcomingEffectProcessors()
        {
            return _effectHolder
                .Effects
                .Where(e => e.EffectData.OutcomingActionProcessor != null)
                .Select(e => e.EffectData.OutcomingActionProcessor);
        }

        private IEnumerable<IActionProcessor> GetIncomingEffectProcessors()
        {
            return _effectHolder
                .Effects
                .Where(e => e.EffectData.IncomingActionProcessor != null)
                .Select(e => e.EffectData.IncomingActionProcessor);
        }

        private TAction ProcessAction<TAction>(TAction action, IEnumerable<IActionProcessor> processors)
            where TAction : BattleAction<TAction>
        {
            var processedAction = action;
            foreach (var processor in processors)
            {
                processedAction = processor.ProcessAction(processedAction);
                if (processedAction == null)
                    throw new ArgumentNullException(nameof(processedAction));
            }
            return processedAction;
        }
    }
}
