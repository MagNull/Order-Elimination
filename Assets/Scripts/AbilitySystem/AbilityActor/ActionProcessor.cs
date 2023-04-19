﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    //TODO-ОБРАБОТКА объеденить список эффектов и взаимодействие с ними у Процессора и Актора. 
    //Либо вынести методы обработки в Актора.
    public class ActionProcessor
    {
        private readonly IEffectHolder _effectHolder;
        //Equipments

        private ActionProcessor(IEffectHolder effectHolder)//equipHolder
        {
            _effectHolder = effectHolder;
            //equip
        }

        public static ActionProcessor Create<TOwner>(TOwner owner) where TOwner : IEffectHolder
            => new ActionProcessor(owner);

        public TAction ProcessOutcomingAction<TAction>(TAction action) where TAction : BattleAction<TAction>
        {
            var processedAction = action;
            foreach (var effect in GetOutcomingActionProcessingEffects<TAction>())
            {
                processedAction = effect.ProcessOutcomingAction(processedAction);
                if (processedAction == null)
                    throw new ArgumentNullException(nameof(processedAction));
                // TODO(Critical) Если IBattleAction будет структурой, default может работать неправильно
                // Возможные решения: 
                // 1. Делать IBattleAction классом
                // 2. Изменить структуру метода на bool TryProcessAction(TAction action, out TAction processedAction)
            }
            return processedAction;
        }

        public TAction ProcessIncomingAction<TAction>(TAction action) where TAction : BattleAction<TAction>
        {
            var processedAction = action;
            foreach (var effect in GetIncomingActionProcessingEffects<TAction>())
            {
                processedAction = effect.ProcessIncomingAction(processedAction);
                if (processedAction == null)
                    throw new ArgumentNullException(nameof(processedAction));
                // TODO(Critical) Если IBattleAction будет структурой, default может работать неправильно
                // Возможные решения: 
                // 1. Делать IBattleAction классом
                // 2. Изменить структуру метода на bool TryProcessAction(TAction action, out TAction processedAction)
            }
            return processedAction;
        }

        private IOutcomingActionProcessingEffect<TAction>[] GetOutcomingActionProcessingEffects<TAction>()
            where TAction : BattleAction<TAction>
        {
            return _effectHolder
                .Effects
                .Select(e => e as IOutcomingActionProcessingEffect<TAction>)
                .Where(e => e != null)
                .ToArray();
        }

        private IIncomingActionProcessingEffect<TAction>[] GetIncomingActionProcessingEffects<TAction>()
            where TAction : BattleAction<TAction>
        {
            return _effectHolder
                .Effects
                .Select(e => e as IIncomingActionProcessingEffect<TAction>)
                .Where(e => e != null)
                .ToArray();
        }
    }
}
