using OrderElimination.AbilitySystem.Conditions;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using System;
using System.Linq;

namespace OrderElimination.Editor
{
    public class CompoundIValueEntryFilter : IValueEntryFilter
    {
        [ShowInInspector]
        public IValueEntryFilter[] Filters { get; set; }

        [ShowInInspector]
        public RequireType RequireType { get; set; }

        public bool IsAllowed(ReflectionExtensions.SerializedMember entry)
        {
            return RequireType switch
            {
                RequireType.All => Filters.All(f => f.IsAllowed(entry)),
                RequireType.Any => Filters.Any(f => f.IsAllowed(entry)),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
