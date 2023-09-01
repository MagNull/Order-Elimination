using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;

namespace OrderElimination.AbilitySystem
{
    public class ChangeEnergyPointsAction : BattleAction<ChangeEnergyPointsAction>
    {
        public enum ModificationType
        {
            Add,
            Remove,
            Set,
            SetToPerRoundValue
            //Clear
        }

        [ShowInInspector, OdinSerialize]
        public EnergyPoint EnergyPointType { get; private set; }

        [ShowInInspector, OdinSerialize]
        public ModificationType Modification { get; private set; }

        [ShowIf("@" + nameof(Modification) + "!=" + nameof(ModificationType) + "." + nameof(ModificationType.SetToPerRoundValue))]
        [ShowInInspector, OdinSerialize]
        public int Value { get; private set; }

        public override ActionRequires ActionRequires => ActionRequires.Target;

        protected override async UniTask<IActionPerformResult> Perform(ActionContext useContext)
        {
            switch (Modification)
            {
                case ModificationType.Add:
                    useContext.ActionTarget.AddEnergyPoints(EnergyPointType, Value);
                    break;
                case ModificationType.Remove:
                    useContext.ActionTarget.RemoveEnergyPoints(EnergyPointType, Value);
                    break;
                case ModificationType.Set:
                    useContext.ActionTarget.SetEnergyPoints(EnergyPointType, Value);
                    break;
                case ModificationType.SetToPerRoundValue:
                    useContext.ActionTarget.SetEnergyPoints(
                        EnergyPointType,
                        useContext.BattleContext.BattleRules.GetEnergyPointsPerRound(EnergyPointType));
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
            clone.EnergyPointType = EnergyPointType;
            clone.Modification = Modification;
            clone.Value = Value;
            return clone;
        }
    }
}
