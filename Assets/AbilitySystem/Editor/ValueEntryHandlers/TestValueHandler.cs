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
            if (entry.MemberValue is ShakeAnimation instance)
            {
                //public modifications
                //instance.ShakingEntity = instance.ShakeTarget switch
                //{
                //    AnimationTarget.Target => ActionEntity.Target,
                //    AnimationTarget.Caster => ActionEntity.Caster,
                //    AnimationTarget.CellGroup => throw new NotImplementedException(),
                //    _ => throw new NotImplementedException(),
                //};
            }
            else throw new InvalidOperationException();
        }
    }
}
