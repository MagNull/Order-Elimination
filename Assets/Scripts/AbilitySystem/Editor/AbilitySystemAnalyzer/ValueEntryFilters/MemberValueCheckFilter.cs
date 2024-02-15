using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using System;
using Unity.VisualScripting;

namespace OrderElimination.Editor
{
    public class MemberValueCheckFilter : IValueEntryFilter
    {
        public enum CompareOption
        {
            Equals,
            NotEquals
        }

        [ShowInInspector]
        public string MemberName { get; set; }

        [ShowInInspector]
        public CompareOption ValueComapsion { get; set; }

        [ShowInInspector]
        public object Value { get; set; }

        public bool IsAllowed(ReflectionExtensions.SerializedMember entry)
        {
            if (entry == null)
                throw new ArgumentNullException();
            if (entry.MemberValue == null) 
                return false;
            if (!entry.MemberValue.HasFieldOrPropertySequence(MemberName, out var memberType, out var value))
                throw new ArgumentException($"Field or Property with name {MemberName} was not found.");
            return ValueComapsion switch
            {
                CompareOption.Equals => value.Equals(Value),
                CompareOption.NotEquals => !value.Equals(Value),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
