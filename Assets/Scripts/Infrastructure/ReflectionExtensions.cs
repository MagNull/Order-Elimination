using OrderElimination.AbilitySystem;
using OrderElimination.AbilitySystem.Animations;
using Sirenix.Serialization;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

namespace OrderElimination.Infrastructure
{
    public static class ReflectionExtensions
    {
        public static bool IsNumericType(this Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsNumeric(this object obj)
        {
            return obj is sbyte
            || obj is byte
            || obj is short
            || obj is ushort
            || obj is int
            || obj is uint
            || obj is long
            || obj is ulong
            || obj is float
            || obj is double
            || obj is decimal;
        }

        public static bool IsAbilitySystemAsset(this object obj)
        {
            return obj is ActiveAbilityBuilder
                || obj is PassiveAbilityBuilder
                || obj is EffectDataPreset
                || obj is CharacterTemplate
                || obj is StructureTemplate;
                //|| obj is AnimationPreset
        }

        //Resource-heavy
        public static Type[] GetAllInterfaceImplementationTypes<T>()
        {
            var type = typeof(T);
            if (!type.IsInterface)
                Logging.LogException( new ArgumentException($"Type {nameof(T)} must be an interface."));
            return Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.GetInterfaces().Contains(type)).ToArray();
        }

        public static bool IsImplementingAllInterfaces(this Type type, params string[] names)
        {
            return names.All(n => type.GetInterface(n) != null);
        }

        public static bool IsImplementingAnyInterface(this Type type, params string[] names)
        {
            return names.Any(n => type.GetInterface(n) != null);
        }

        public static string GetExceptionName(this Exception exception)
            => exception.GetType().Name;

        public static bool HasSerializationAttributes(MemberInfo m)
            => Attribute.IsDefined(m, typeof(OdinSerializeAttribute))
            || Attribute.IsDefined(m, typeof(SerializableAttribute))
            || Attribute.IsDefined(m, typeof(SerializeField))
            || Attribute.IsDefined(m, typeof(SerializeReference));

        public static IEnumerable<object> GetSerializedNonCollectionInstances(object instance)//Unwraps collections
        {
            if (instance == null)
                return Enumerable.Empty<object>();

            var instanceType = instance.GetType();

            if (instanceType.IsPrimitive)
                return Enumerable.Empty<object>();

            var bindingFlags =
                BindingFlags.Instance
                | BindingFlags.Public
                | BindingFlags.NonPublic
                | BindingFlags.SetField
                | BindingFlags.SetProperty;

            var fields = instanceType.GetFields(bindingFlags).Where(HasSerializationAttributes);
            var properties = instanceType.GetProperties(bindingFlags).Where(HasSerializationAttributes);

            var serializedValues = new List<object>();

            foreach (var member in fields.AsEnumerable<MemberInfo>().Concat(properties))
            {
                var value = member.GetMemberValue(instance);
                if (value == null) continue;
                serializedValues.Add(value);
                if (value is ICollection collection)
                {
                    if (collection is IDictionary dictionary)
                    {
                        serializedValues.AddRange(
                            dictionary.Keys.AsEnumerable()
                            .Concat(dictionary.Values.AsEnumerable())
                            .Where(e => e != null));
                    }
                    else
                    {
                        serializedValues.AddRange(
                            collection.AsEnumerable()
                            .Where(e => e != null));
                    }
                }
            }
            return serializedValues;
        }

        public static IEnumerable<SerializedMember> GetSerializedMembers(object obj)
        {
            if (obj == null)
                return Enumerable.Empty<SerializedMember>();
            var instanceType = obj.GetType();
            if (instanceType.IsPrimitive)
                return Enumerable.Empty<SerializedMember>();

            var bindingFlags =
                BindingFlags.Instance
                | BindingFlags.Public
                | BindingFlags.NonPublic
                | BindingFlags.SetField
                | BindingFlags.SetProperty;

            var fields = instanceType.GetFields(bindingFlags).Where(HasSerializationAttributes);
            var properties = instanceType.GetProperties(bindingFlags).Where(HasSerializationAttributes);

            return fields
                .AsEnumerable<MemberInfo>()
                .Concat(properties)
                .Select(m => new SerializedMember(m, obj))
                .ToArray();
        }

        public static IEnumerable<SerializedMember> GetSerializedMembers(SerializedMember member)
        {
            if (member.MemberValue == null)
                return Enumerable.Empty<SerializedMember>();
            var instanceType = member.MemberValue.GetType();
            if (instanceType.IsPrimitive)
                return Enumerable.Empty<SerializedMember>();

            var bindingFlags =
                BindingFlags.Instance
                | BindingFlags.Public
                | BindingFlags.NonPublic
                | BindingFlags.SetField
                | BindingFlags.SetProperty;

            var fields = instanceType.GetFields(bindingFlags).Where(HasSerializationAttributes);
            var properties = new List<MemberInfo>(); 
            foreach (var p in instanceType.GetProperties(bindingFlags).Where(HasSerializationAttributes))
            {
                //Debug.Log($"Checking {p.Name}");
                properties.Add(p);
            }

            return fields
                .AsEnumerable<MemberInfo>()
                .Concat(properties)
                .Select(m => new SerializedMember(m, member.MemberValue, member))
                .ToArray();
        }

        public static IEnumerable<SerializedMember> GetAllSerializedMembersOfType(
            SerializedMember parent, Type seekingType, Func<object, bool> stopAtInstance)
        {
            if (parent == null || seekingType == null)
                throw new ArgumentNullException();
            var membersOfType = new List<SerializedMember>();
            foreach (var e in GetSerializedMembers(parent))
            {
                var value = e.MemberValue;
                if (value == null) continue;//Add option to include null values
                if (seekingType.IsAssignableFrom(value.GetType()))
                    membersOfType.Add(e);
                if (stopAtInstance != null && stopAtInstance(value))
                    continue;
                //Debug.Log($"Checking {e.GetFullName()}");
                if (value is ICollection collection)
                {
                    object[] collectionElements;
                    if (value is IDictionary dictionary)
                    {
                        collectionElements = 
                            dictionary.Keys.AsEnumerable()
                            .Concat(dictionary.Values.AsEnumerable())
                            .ToArray();
                    }
                    else
                    {
                        collectionElements = collection.AsEnumerable().ToArray();
                    }
                    foreach (var x in collectionElements
                        .Where(x => x != null)
                        .Select(x => new SerializedMember($"{e.Member.Name}[i]", x, parent)))
                    {
                        if (seekingType.IsAssignableFrom(x.MemberValue.GetType()))
                            membersOfType.Add(x);
                        if (stopAtInstance != null && stopAtInstance(x.MemberValue))
                            continue;
                        membersOfType.AddRange(GetAllSerializedMembersOfType(x, seekingType, stopAtInstance));
                    }
                }
                membersOfType.AddRange(GetAllSerializedMembersOfType(e, seekingType, stopAtInstance));
            }
            return membersOfType;
        }

        public class SerializedMember
        {
            public SerializedMember(MemberInfo member, object memberOwner) : this(member, memberOwner, null)
            {

            }

            public SerializedMember(MemberInfo member, object memberOwner, SerializedMember parent)
            {
                SerializedParent = parent;
                Member = member;
                MemberDefinedType = member.MemberType switch
                {
                    MemberTypes.Field => ((FieldInfo)member).FieldType,
                    MemberTypes.Property => ((PropertyInfo)member).PropertyType,
                    _ => null
                };
                if (member is FieldInfo field)
                    MemberDefinedType = field.FieldType;
                else if (member is PropertyInfo property)
                    MemberDefinedType = property.PropertyType;
                MemberValue = member.GetMemberValue(memberOwner);
                MemberOwner = memberOwner;
            }

            public SerializedMember(string memberNameReplacement, object memberValue, SerializedMember parent)
            {
                SerializedParent = parent;
                Member = null;
                MemberValue = memberValue;
                MemberOwner = null;
                MemberDefinedType = null;
                MemberNameReplacement = memberNameReplacement;
            }

            public MemberInfo Member { get; }
            public Type MemberDefinedType { get; }
            public object MemberValue { get; }
            public object MemberOwner { get; }
            public SerializedMember SerializedParent { get; }
            public string MemberNameReplacement { get; } = "???";

            public string GetFullName()
            {
                var parent = SerializedParent != null ? $"{SerializedParent.GetFullName()}." : "";
                return $"{parent}{Member?.Name ?? MemberNameReplacement}";
            }
        }
    }
}
