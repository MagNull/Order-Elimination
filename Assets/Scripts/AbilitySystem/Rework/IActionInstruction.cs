using System;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.AbilitySystem.Rework
{
    public interface IActionInstruction
    {
        public ITargetCondition[] ActionCondition { get; set; }
        //TODO Action нужно дублировать перед обработкой
        public IBattleAction Action { get; set; }
        //При каждом успешном выполнении Action будут вызываться последующие инструкции (для каждого повторения)
        public int RepeatNumber { get; set; }
        public event Action<InstructionPerformContext> PerformedSuccessfully; 
        public event Action<InstructionPerformContext> FailedToPerform; 
    }

    public struct InstructionPerformContext
    {
        public AbilityRunner RelatedAbility;
        public IBattleAction Action;
        public IAbilitySystemActor Caster;
        public IAbilitySystemActor Target;
        public Vector2Int TargetPosition;
    }
}
