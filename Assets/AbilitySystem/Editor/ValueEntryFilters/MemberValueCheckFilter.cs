using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using System;

namespace OrderElimination.Editor
{
    public class MemberValueCheckFilter : IValueEntryFilter
    {
        [ShowInInspector]
        public MemberTypeOption MemberType { get; set; }

        [ShowInInspector]
        public string MemberName { get; set; }

        [ShowInInspector]
        public object Value { get; set; }

        public bool IsAllowed(ReflectionExtensions.SerializedMember entry)
        {
            if (entry == null)
                throw new ArgumentNullException();
            if (entry.MemberValue == null) 
                return false;
            var type = entry.MemberValue.GetType();
            if (MemberType == MemberTypeOption.Field)
            {
                var field = type.GetField(MemberName);
                if (field == null)
                    throw new ArgumentException($"Field with name {MemberName} was not found in {type.Name}");
                var value = field.GetValue(entry.MemberValue);
                return value.Equals(Value);
            }
            else if (MemberType == MemberTypeOption.Property)
            {
                var property = type.GetProperty(MemberName); 
                if (property == null)
                    throw new ArgumentException($"Property with name {MemberName} was not found in {type.Name}");
                var value = property.GetValue(entry.MemberValue);
                return value.Equals(Value);
            }
            else throw new NotImplementedException();
        }
    }
}
