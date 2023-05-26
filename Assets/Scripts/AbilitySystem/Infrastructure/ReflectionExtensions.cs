using OrderElimination.AbilitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

        public static Type[] GetAllInterfaceImplementationTypes<T>()
        {
            var type = typeof(T);
            if (!type.IsInterface)
                throw new ArgumentException($"Type {nameof(T)} must be an interface.");
            return Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.GetInterfaces().Contains(type)).ToArray();
        }
    }
}
