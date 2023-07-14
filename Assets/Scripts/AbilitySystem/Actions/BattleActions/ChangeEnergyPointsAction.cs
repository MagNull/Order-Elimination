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
    public class ChangeEnergyPointsAction : BattleAction<ChangeEnergyPointsAction>
    {
        public enum ModificationType
        {
            Add,
            Remove,
            Set,
            //Clear
        }

        [ShowInInspector, OdinSerialize]
        public EnergyPoint EnergyPoint { get; private set; }

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
                    useContext.ActionTarget.AddEnergyPoints(EnergyPoint, Value);
                    break;
                case ModificationType.Remove:
                    useContext.ActionTarget.RemoveEnergyPoints(EnergyPoint, Value);
                    break;
                case ModificationType.Set:
                    useContext.ActionTarget.SetEnergyPoints(EnergyPoint, Value);
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
            var clone = new ChangeEnergyPointsAction();
            clone.EnergyPoint = EnergyPoint;
            clone.Modification = Modification;
            clone.Value = Value;
            return clone;
        }
    }
}
