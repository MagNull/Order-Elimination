using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using System;

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
        public MemberTypeOption MemberType { get; set; }

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
            var type = entry.MemberValue.GetType();
            object value;
            if (MemberType == MemberTypeOption.Field)
            {
                var field = type.GetField(MemberName);
                if (field == null)
                    throw new ArgumentException($"Field with name {MemberName} was not found in {type.Name}");
                value = field.GetValue(entry.MemberValue);
            }
            else if (MemberType == MemberTypeOption.Property)
            {
                var property = type.GetProperty(MemberName); 
                if (property == null)
                    throw new ArgumentException($"Property with name {MemberName} was not found in {type.Name}");
                value = property.GetValue(entry.MemberValue);
            }
            else throw new NotImplementedException();
            return ValueComapsion switch
            {
                CompareOption.Equals => value.Equals(Value),
                CompareOption.NotEquals => !value.Equals(Value),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
