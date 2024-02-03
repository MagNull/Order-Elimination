using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;

namespace OrderElimination.MacroGame
{
    public interface IStatUpgradeRules
    {
        public float MaxUpgradeLevel { get; }

        public float GetUpgradeToLevelCost(BattleStat stat, float upgradeLevel);

        public ValueModifier GetStatModifier(BattleStat stat, float upgradeLevel);

        public float GetEstimatedUpgradeLevel(BattleStat stat, ValueModifier modifier);
    }
}
