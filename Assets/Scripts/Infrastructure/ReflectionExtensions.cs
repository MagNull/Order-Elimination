using OrderElimination.AbilitySystem;
using OrderElimination.AbilitySystem.Animations;
using Sirenix.Serialization;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                || obj is StructureTemplate
                || obj is AnimationPreset;
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

        public static IEnumerable<object> GetSerializedMembers(object instance)
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
            Func<MemberInfo, bool> HasRequiredAttributes = m =>
            Attribute.IsDefined(m, typeof(OdinSerializeAttribute))
            || Attribute.IsDefined(m, typeof(SerializableAttribute))
            || Attribute.IsDefined(m, typeof(SerializeField))
            || Attribute.IsDefined(m, typeof(SerializeReference));

            var fields = instanceType.GetFields(bindingFlags).Where(HasRequiredAttributes).ToArray();
            var properties = instanceType.GetProperties(bindingFlags).Where(HasRequiredAttributes).ToArray();

            var nextValues = new List<object>();

            foreach (var member in fields.Concat(properties))
            {
                var value = member.GetMemberValue(instance);
                if (value == null) continue;
                nextValues.Add(value);
                if (value is ICollection collection)
                {
                    if (collection is IDictionary dictionary)
                    {
                        nextValues.AddRange(
                            dictionary.Keys.AsEnumerable()
                            .Concat(dictionary.Values.AsEnumerable())
                            .Where(e => e != null));
                    }
                    else
                    {
                        nextValues.AddRange(
                            collection.AsEnumerable()
                            .Where(e => e != null));
                    }
                }
            }
            return nextValues;
        }
    }
}
