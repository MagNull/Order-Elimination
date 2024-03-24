using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using System;

namespace OrderElimination.Editor
{
    public class MemberContainsFilter : IValueEntryFilter
    {
        [ShowInInspector]
        public string MemberName { get; set; } = string.Empty;

        public bool IsAllowed(ReflectionExtensions.SerializedMember entry)
        {
            if (entry == null)
                throw new ArgumentNullException();
            if (entry.MemberValue == null)
                return false;
            return entry.MemberValue.HasFieldOrPropertySequence(MemberName, out var memberType, out var value);
        }
    }
}
