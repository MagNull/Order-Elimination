using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;

namespace OrderElimination.AbilitySystem
{
    [PropertyTooltip("@$value." + nameof(CallbackDescription))]
    public interface ICallbackingBattleAction : IBattleAction
    {
        public string CallbackDescription { get; }

        public UniTask<IActionPerformResult> ModifiedPerformWithCallbacks(
            ActionContext useContext,
            Action<IBattleActionCallback> onCallback,
            bool actionMakerProcessing = true,
            bool targetProcessing = true);
    }

    public interface IBattleActionCallback
    {

    }
}
