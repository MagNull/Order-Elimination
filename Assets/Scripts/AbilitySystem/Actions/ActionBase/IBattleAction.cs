using Cysharp.Threading.Tasks;
using OrderElimination.Infrastructure;
using System;

namespace OrderElimination.AbilitySystem
{
    [Obsolete("Интерфейс " + nameof(IBattleAction) + " является обобщающим. По возможности используйте BattleAction<TAction>.")]
    public interface IBattleAction : ICloneable<IBattleAction>
    {
        public ActionRequires ActionRequires { get; }

        //public bool CanPerform(ActionExecutionContext useContext, bool actionMakerProcessing = true, bool targetProcessing = true);

        public event Action<IActionPerformResult> SuccessfullyPerformed;
        public event Action<IActionPerformResult> FailedToPerformed;
        //public int RepeatNumber
        public UniTask<IActionPerformResult> ModifiedPerform(
            ActionContext useContext, 
            bool actionMakerProcessing = true,
            bool targetProcessing = true);
    }
}
