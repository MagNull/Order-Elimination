using OrderElimination.AbilitySystem;
using OrderElimination.AbilitySystem.Animations;
using OrderElimination.Infrastructure;
using System;
using UnityEngine;

namespace OrderElimination.Editor
{
    public class TestValueHandler : IValueEntryHandler
    {
        public void HandleValueEntry(ReflectionExtensions.SerializedMember entry)
        {
            if (entry.MemberValue is EmptyAction instance)
            {
                //public modifications
                //instance.ActionType = (BattleActionType)(int)instance.ActionTypeS;
                //instance.ActionTypeS = (ActionTypeC)(int)instance.ExecutesOn;
            }
            else throw new InvalidOperationException();
        }
    }
}
