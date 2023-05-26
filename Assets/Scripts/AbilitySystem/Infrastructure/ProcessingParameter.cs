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

        public event Action<ProcessingParameter<TValue>> ValueChanged;

        private readonly List<Func<TValue, TValue>> modifyingFormulas = new List<Func<TValue, TValue>>();

        public ProcessingParameter(TValue initialValue = default)
        {
            ModifiedValue = UnmodifiedValue = initialValue;
        }

        public void AddProcessor(Func<TValue, TValue> processor)
        {
            modifyingFormulas.Add(processor);
            ModifiedValue = GetRecalculatedModifiedValue();
            ValueChanged?.Invoke(this);
        }

        public bool RemoveProcessor(Func<TValue, TValue> processor)
        {
            var wasRemoved = modifyingFormulas.Remove(processor);
            if (wasRemoved)
            {
                ModifiedValue = GetRecalculatedModifiedValue();
                ValueChanged?.Invoke(this);
            }
            return wasRemoved;
        }

        public void RemoveAllProcessors()
        {
            if (modifyingFormulas.Count > 0)
            {
                modifyingFormulas.Clear();
                ModifiedValue = GetRecalculatedModifiedValue();
                ValueChanged?.Invoke(this);
            }
        }

        public void SetUnmodifiedValue(TValue value)
        {
            UnmodifiedValue = value;
            ModifiedValue = GetRecalculatedModifiedValue();
            ValueChanged?.Invoke(this);
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
