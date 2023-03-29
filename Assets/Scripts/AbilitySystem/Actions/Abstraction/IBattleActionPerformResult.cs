using OrderElimination.AbilitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public interface IBattleActionPerformResult<TAction> where TAction : IBattleAction
    {
        public TAction Action { get; }
        public bool IsPerformedSuccessfully { get; }
        //PerformFailReason
        //NewGeneratedActions
    }
}
