using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
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
        public string MemberName { get; set; } = string.Empty;

        [ShowInInspector]
        public CompareOption ValueComparison { get; set; }

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
            return ValueComparison switch
            {
                CompareOption.Equals => value.Equals(Value),
                CompareOption.NotEquals => !value.Equals(Value),
                _ => throw new NotImplementedException(),
            };
        }

        [Button(Style = ButtonStyle.Box)]
        private void TryCreateNewInstance(Type objectType)
        {
            try
            {
                var obj = Activator.CreateInstance(objectType);
                Value = obj;
            }
            catch
            {
                throw;
            }
        }
    }
}
