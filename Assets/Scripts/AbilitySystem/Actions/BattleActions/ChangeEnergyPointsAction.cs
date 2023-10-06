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

        [EnableIf("@" + nameof(Modification) + "!=" + nameof(ModificationType) + "." + nameof(ModificationType.SetToPerRoundValue))]
        [ShowInInspector, OdinSerialize]
        public int Value { get; private set; }

        public override BattleActionType BattleActionType => BattleActionType.EntityAction;

        protected override async UniTask<IActionPerformResult> Perform(ActionContext useContext)
        {
            switch (Modification)
            {
                case ModificationType.Add:
                    useContext.TargetEntity.AddEnergyPoints(EnergyPointType, Value);
                    break;
                case ModificationType.Remove:
                    useContext.TargetEntity.RemoveEnergyPoints(EnergyPointType, Value);
                    break;
                case ModificationType.Set:
                    useContext.TargetEntity.SetEnergyPoints(EnergyPointType, Value);
                    break;
                case ModificationType.SetToPerRoundValue:
                    useContext.TargetEntity.SetEnergyPoints(
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
