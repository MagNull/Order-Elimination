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
        [TitleGroup("Battle End Trackers")]
        [BoxGroup("Battle End Trackers/Victory Tracker", ShowLabel = false)]
        [GUIColor(0.6f, 1f, 0.3f)]
        [ShowInInspector, OdinSerialize]
        public IBattleTracker VictoryTracker { get; set; }

        [TitleGroup("Battle End Trackers")]
        [BoxGroup("Battle End Trackers/Defeat Tracker", ShowLabel = false)]
        [GUIColor(1f, 0.4f, 0.3f)]
        [ShowInInspector, OdinSerialize]
        public IBattleTracker DefeatTracker { get; set; }

        [TitleGroup("Battle Parameters")]
        [ShowInInspector, OdinSerialize]
        public ITurnPriority TurnPriority { get; set; } = new PlayerFirstTurnPriority();

        [TitleGroup("Battle Parameters")]
        [ShowInInspector, OdinSerialize]
        public IHitCalculation HitCalculation { get; set; } = new StandardHitCalculation();

        [TitleGroup("Energy Points")]
        [ShowInInspector, OdinSerialize]
        public int MovementPointsPerRound { get; set; } = 1;

        [TitleGroup("Energy Points")]
        [ShowInInspector, OdinSerialize]
        public int AttackPointsPerRound { get; set; } = 1;

        [TitleGroup("Energy Points")]
        [ShowInInspector, OdinSerialize]
        public int ConsumablesPointsPerRound { get; set; } = 1;

        [TitleGroup("Energy Points")]
        [PropertyTooltip("Resets points to default value instead of adding them.")]
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
