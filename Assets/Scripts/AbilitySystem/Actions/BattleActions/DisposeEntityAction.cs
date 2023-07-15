using Cysharp.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public class DisposeEntityAction : BattleAction<DisposeEntityAction>//, IUndoableBattleAction
    {
        //Where to place entity on undo?

        public override ActionRequires ActionRequires => ActionRequires.Target;

        public override IBattleAction Clone()
        {
            var clone = new DisposeEntityAction();
            return clone;
        }

        protected override async UniTask<IActionPerformResult> Perform(ActionContext useContext)
        {
            var result = useContext.ActionTarget.DisposeFromBattle();
            return new SimplePerformResult(this, useContext, result);
        }
    }
}
