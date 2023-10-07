using Cysharp.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public class DisposeEntityAction : BattleAction<DisposeEntityAction>//, IUndoableBattleAction
    {
        //Where to place entity on undo?

        public override BattleActionType BattleActionType => BattleActionType.EntityAction;

        public override IBattleAction Clone()
        {
            var clone = new DisposeEntityAction();
            return clone;
        }

        protected override async UniTask<IActionPerformResult> Perform(ActionContext useContext)
        {
            var result = useContext.TargetEntity.DisposeFromBattle();
            return new SimplePerformResult(this, useContext, result);
        }
    }
}
