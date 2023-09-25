using OrderElimination.AbilitySystem;

namespace OrderElimination.Battle
{
    public interface IBattleRules
    {
        public ITurnPriority TurnPriority { get; }
        public IHitCalculation HitCalculation { get; }

        public IBattleTracker VictoryTracker { get; }
        public IBattleTracker DefeatTracker { get; }
        //VictoryConditions (All/Any-compound)
        //DefeatConditions (Any)

        public int MovementPointsPerRound { get; }
        public int AttackPointsPerRound { get; }
        public int ConsumablesPointsPerRound { get; }
        public bool HardResetEnergyPointsEveryRound { get; }//Reset to initial value instead of adding when needed

        public int GetEnergyPointsPerRound(EnergyPoint pointType);
        //public void SetEnergyPointsPerRound(EnergyPoint pointType, int valuePerRound);
    }
}
