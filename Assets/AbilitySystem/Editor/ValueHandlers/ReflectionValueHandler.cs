using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.Editor
{
    public class ReflectionValueHandler : IValueEntryHandler
    {
        public void HandleValueEntry(ReflectionExtensions.SerializedMember entry)
        {
            if (entry.MemberValue is EntitiesCountGetter countGetter && countGetter != null)
            {
                //reflection modifications
                var property = typeof(EntitiesCountGetter)
                    .GetProperty(nameof(EntitiesCountGetter.CountInCellGroupId));
                dynamic value = property.GetValue(entry.MemberValue);
                property.SetValue(entry.MemberValue, value + 1);
            }
            else throw new InvalidOperationException();
        }
    }
}
