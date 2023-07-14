using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public readonly struct ContextValueModificationResult
    {
        public readonly bool IsModificationSuccessful;
        public readonly IContextValueGetter ModifiedValueGetter;

        public ContextValueModificationResult(bool isModified, IContextValueGetter modifiedValueGetter)
        {
            IsModificationSuccessful = isModified;
            ModifiedValueGetter = modifiedValueGetter;
        }
    }
}
