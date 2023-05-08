using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public class ChangeStatusAction : BattleAction<ChangeStatusAction>
    {
        public enum StatusAction
        {
            Increase,
            Decrease,
            //Clear
        }

        [ShowInInspector, OdinSerialize]
        public BattleStatus Status { get; private set; }

        [ShowInInspector, OdinSerialize]
        public StatusAction StatusOperation { get; private set; }

        public override ActionRequires ActionRequires => ActionRequires.Entity;

        public override IBattleAction Clone()
        {
            var clone = new ChangeStatusAction();
            clone.Status = Status;
            clone.StatusOperation = StatusOperation;
            return clone;
        }

        protected async override UniTask<bool> Perform(ActionContext useContext)
        {
            switch (StatusOperation)
            {
                case StatusAction.Increase:
                    useContext.ActionTarget.StatusHolder.IncreaseStatus(Status);
                    return true;
                case StatusAction.Decrease:
                    return useContext.ActionTarget.StatusHolder.DecreaseStatus(Status);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
