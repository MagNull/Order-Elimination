using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.AbilitySystem
{
    public interface ICallbackingBattleAction : IBattleAction
    {
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
