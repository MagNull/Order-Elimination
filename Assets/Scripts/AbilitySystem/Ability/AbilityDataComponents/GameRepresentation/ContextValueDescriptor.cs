using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.AbilitySystem
{
    public enum ContextValueTag
    {
        //Placeholder value will be passed during runtime (e.g. incoming damage)
        //It is impossible to know tags for such value, so little sense to make such tag.
        //It could be used only in processors.
        //IsPlaceholderValue ???
        IsRandom = 1,
        IsBattleOnly = 2,
        DependsOnCaster,
        DependsOnTarget,
        DependsOnCellGroups
    }

    public class ContextValueDescriptor
    {
        public EnumMask<ContextValueTag> Tags { get; private set; } //inherits from children

        private IContextValueGetter ValueGetter { get; set; }
    }
}
