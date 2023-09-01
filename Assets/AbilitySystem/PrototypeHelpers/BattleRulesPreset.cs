using OrderElimination.AbilitySystem;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using UnityEngine;

namespace OrderElimination.Battle
{
    [CreateAssetMenu(fileName = "new BattleRulesPreset", menuName = "OrderElimination/Battle/Rules Preset")]
    public class BattleRulesPreset : SerializedScriptableObject, IBattleRules
    {
        [ShowInInspector, OdinSerialize]
        public ITurnPriority TurnPriority { get; set; } = new PlayerFirstTurnPriority();

        [ShowInInspector, OdinSerialize]
        public IHitCalculation HitCalculation { get; set; } = new StandardHitCalculation();

        [ShowInInspector, OdinSerialize]
        public int MovementPointsPerRound { get; set; } = 1;

        [ShowInInspector, OdinSerialize]
        public int AttackPointsPerRound { get; set; } = 1;

        [ShowInInspector, OdinSerialize]
        public int ConsumablesPointsPerRound { get; set; } = 1;

        [ShowInInspector, OdinSerialize]
        public bool HardResetEnergyPointsEveryRound { get; set; } = true;

        public int GetEnergyPointsPerRound(EnergyPoint pointType)
        {
            return pointType switch
            {
                EnergyPoint.MovementPoint => MovementPointsPerRound,
                EnergyPoint.AttackPoint => AttackPointsPerRound,
                EnergyPoint.ConsumablesPoint => ConsumablesPointsPerRound,
                _ => throw new NotImplementedException(),
            };
        }

        public void SetEnergyPointsPerRound(EnergyPoint pointType, int valuePerRound)
        {
            switch (pointType)
            {
                case EnergyPoint.MovementPoint:
                    MovementPointsPerRound = valuePerRound;
                    break;
                case EnergyPoint.AttackPoint:
                    AttackPointsPerRound = valuePerRound;
                    break;
                case EnergyPoint.ConsumablesPoint:
                    ConsumablesPointsPerRound = valuePerRound;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
