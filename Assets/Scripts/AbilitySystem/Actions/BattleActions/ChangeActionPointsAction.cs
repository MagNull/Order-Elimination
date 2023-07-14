using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public class ChangeActionPointsAction : BattleAction<ChangeActionPointsAction>
    {
        public enum ModificationType
        {
            Add,
            Remove,
            Set,
            //Clear
        }

        [ShowInInspector, OdinSerialize]
        public ActionPoint ActionPoint { get; private set; }

        [ShowInInspector, OdinSerialize]
        public ModificationType Modification { get; private set; }

        [ShowInInspector, OdinSerialize]
        public int Value { get; private set; }

        public override ActionRequires ActionRequires => ActionRequires.Target;

        protected override async UniTask<IActionPerformResult> Perform(ActionContext useContext)
        {
            switch (Modification)
            {
                case ModificationType.Add:
                    useContext.ActionTarget.AddActionPoints(ActionPoint, Value);
                    break;
                case ModificationType.Remove:
                    useContext.ActionTarget.RemoveActionPoints(ActionPoint, Value);
                    break;
                case ModificationType.Set:
                    useContext.ActionTarget.SetActionPoints(ActionPoint, Value);
                    break;
                //case ModificationType.Clear:
                //    useContext.ActionTarget.ClearActionPoints(ActionPoint);
                //    break;
                default:
                    throw new NotImplementedException();
            }
            return new SimplePerformResult(this, useContext, true);
        }

        public override IBattleAction Clone()
        {
            var clone = new ChangeActionPointsAction();
            clone.ActionPoint = ActionPoint;
            clone.Modification = Modification;
            clone.Value = Value;
            return clone;
        }
    }
}
