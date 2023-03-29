using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.Infrastructure
{
    public class ProcessingParameter<TValue>
    {
        /// <summary>
        /// Change of this value can NOT be reverted.
        /// </summary>
        public TValue UnmodifiedValue { get; private set; } = default;
        /// <summary>
        /// Change of this value can be reverted.
        /// </summary>
        public TValue ModifiedValue { get; private set; } = default;

        private readonly List<Func<TValue, TValue>> modifyingFormulas = new List<Func<TValue, TValue>>();

        public ProcessingParameter(TValue initialValue = default)
        {
            ModifiedValue = UnmodifiedValue = initialValue;
        }

        public void AddProcessor(Func<TValue, TValue> processor)
        {
            modifyingFormulas.Add(processor);
            ModifiedValue = GetRecalculatedModifiedValue();
        }

        public bool RemoveProcessor(Func<TValue, TValue> processor)
        {
            var wasRemoved = modifyingFormulas.Remove(processor);
            ModifiedValue = GetRecalculatedModifiedValue();
            return wasRemoved;
        }

        public void RemoveAllProcessors()
        {
            modifyingFormulas.Clear();
            ModifiedValue = GetRecalculatedModifiedValue();
        }

        public void SetUnmodifiedValue(TValue value)
        {
            UnmodifiedValue = value;
            ModifiedValue = GetRecalculatedModifiedValue();
        }

        private TValue GetRecalculatedModifiedValue()
        {
            var modifiedValue = UnmodifiedValue;
            foreach (var f in modifyingFormulas)
            {
                modifiedValue = f(modifiedValue);
            }
            return modifiedValue;
        }
    }
}
