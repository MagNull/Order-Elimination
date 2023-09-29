using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using System;

namespace OrderElimination.Editor
{
    public class MemberValueSetter : IValueEntryHandler
    {
        [ShowInInspector]
        public MemberTypeOption MemberType { get; set; }

        [ShowInInspector]
        public string MemberName { get; set; }

        [ShowInInspector]
        public object Value { get; set; }

        public void HandleValueEntry(ReflectionExtensions.SerializedMember entry)
        {
            if (entry == null)
                throw new ArgumentNullException();
            if (entry.MemberValue == null)
                throw new ArgumentNullException();
            var type = entry.MemberValue.GetType();
            var newValueType = Value.GetType();
            if (MemberType == MemberTypeOption.Field)
            {
                var field = type.GetField(MemberName);
                if (field == null)
                    throw new ArgumentException($"Field with name {MemberName} was not found in {type.Name}");
                var fieldType = field.FieldType;
                if (!fieldType.IsAssignableFrom(newValueType))
                    throw new InvalidCastException($"{fieldType.Name} is not assignable from {newValueType.Name}");
                field.SetValue(entry.MemberValue, Value);
            }
            else if (MemberType == MemberTypeOption.Property)
            {
                var property = type.GetProperty(MemberName);
                if (property == null)
                    throw new ArgumentException($"Property with name {MemberName} was not found in {type.Name}");
                var propertyType = property.PropertyType;
                if (!propertyType.IsAssignableFrom(newValueType))
                    throw new InvalidCastException($"{propertyType.Name} is not assignable from {newValueType.Name}");
                property.SetValue(entry.MemberValue, Value);
            }
        }
    }
}
