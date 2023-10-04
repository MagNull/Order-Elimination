using Cysharp.Threading.Tasks;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace OrderElimination.AbilitySystem
{
    public class EmptyAction : BattleAction<EmptyAction>
    {
        [ShowInInspector, OdinSerialize]
        public BattleActionType ActionType { get; set; } = BattleActionType.CellAction;

        [DisableIf("@" + nameof(HasRandomSuccessChance))]
        [ShowInInspector, OdinSerialize]
        public bool IsSuccessful { get; set; } = true;

        [ShowInInspector, OdinSerialize]
        public bool HasRandomSuccessChance { get; set; } = false;

        [ShowIf("@" + nameof(HasRandomSuccessChance))]
        [ShowInInspector, OdinSerialize]
        public IContextValueGetter SuccessChance { get; set; } = new ConstValueGetter(0.5f);


        public override BattleActionType BattleActionType => ActionType;

        public override IBattleAction Clone()
        {
            var clone = new EmptyAction();
            clone.ActionType = ActionType;
            clone.HasRandomSuccessChance = HasRandomSuccessChance;
            clone.SuccessChance = SuccessChance != null ? SuccessChance.Clone() : null;
            clone.IsSuccessful = IsSuccessful;
            return clone;
        }

        protected async override UniTask<IActionPerformResult> Perform(ActionContext useContext)
        {
            var success = IsSuccessful; 
            if (HasRandomSuccessChance)
            {
                var chance = SuccessChance.GetValue(ValueCalculationContext.Full(useContext));
                success = RandomExtensions.RollChance(chance);
            }
            return new SimplePerformResult(this, useContext, success);
        }
    }
}
