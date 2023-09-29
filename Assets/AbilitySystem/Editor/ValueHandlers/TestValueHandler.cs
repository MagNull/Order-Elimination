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
            if (entry.MemberValue is EntitiesCountGetter countGetter)
            {
                //public modifications
            }
            else throw new InvalidOperationException();
        }
    }
}
