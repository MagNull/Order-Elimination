using OrderElimination.AbilitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    //Нет смысла делать результат дженериком от TAction, его всё равно не привести к конечному типу
    public interface IBattleActionPerformResult<TAction> where TAction : BattleAction<TAction>
    {
        public TAction Action { get; }
        public bool IsPerformedSuccessfully { get; }
        //PerformFailReason
        //NewGeneratedActions
    }

    public class DamageInflictPerformResult : IBattleActionPerformResult<InflictDamageAction>
    {
        public InflictDamageAction Action => throw new System.NotImplementedException();
        public bool IsPerformedSuccessfully => throw new System.NotImplementedException();
        public DamageInfo DealtDamage => throw new System.NotImplementedException();
    }
}
