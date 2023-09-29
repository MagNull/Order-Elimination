using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Reflection;

namespace OrderElimination.Editor
{
    public class MemberValueCopyHandler : IValueEntryHandler
    {
        [ShowInInspector]
        public MemberTypeOption MemberType { get; set; }

        [ShowInInspector]
        public string MemberName { get; set; }

        [ShowInInspector]
        public MemberTypeOption MemberTypeToCopyFrom { get; set; }

        [ShowInInspector]
        public string MemberNameToCopyFrom { get; set; }

        public void HandleValueEntry(ReflectionExtensions.SerializedMember entry)
        {
            if (entry == null)
                throw new ArgumentNullException();
            if (entry.MemberValue == null)
                throw new ArgumentNullException();
            var type = entry.MemberValue.GetType();
            MemberInfo copyMember = MemberTypeToCopyFrom switch
            {
                MemberTypeOption.Field => GetField(type, MemberNameToCopyFrom),
                MemberTypeOption.Property => GetProperty(type, MemberNameToCopyFrom),
                _ => throw new NotImplementedException(),
            };
            var copyValue = copyMember.GetMemberValue(entry.MemberValue);
            var newValueType = copyValue.GetType();
            if (MemberType == MemberTypeOption.Field)
            {
                var field = GetField(type, MemberName);
                var fieldType = field.FieldType;
                if (!fieldType.IsAssignableFrom(newValueType))
                    throw new InvalidCastException($"{fieldType.Name} is not assignable from {newValueType.Name}");
                field.SetValue(entry.MemberValue, copyValue);
            }
            else if (MemberType == MemberTypeOption.Property)
            {
                var property = GetProperty(type, MemberName);
                var propertyType = property.PropertyType;
                if (!propertyType.IsAssignableFrom(newValueType))
                    throw new InvalidCastException($"{propertyType.Name} is not assignable from {newValueType.Name}");
                property.SetValue(entry.MemberValue, copyValue);
            }
        }

        private FieldInfo GetField(Type where, string memberName)
        {
            var field = where.GetField(memberName);
            if (field == null)
                throw new ArgumentException($"Field with name {memberName} was not found in {where.Name}");
            return field;
        }

        private PropertyInfo GetProperty(Type where, string memberName)
        {
            var property = where.GetProperty(memberName);
            if (property == null)
                throw new ArgumentException($"Property with name {memberName} was not found in {where.Name}");
            return property;
        }
    }
}
