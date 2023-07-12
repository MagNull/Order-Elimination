using OrderElimination.AbilitySystem;

namespace OrderElimination.MacroGame
{
    public interface IReadOnlyGameCharacterStats
    {
        public float MaxHealth { get; }

        public float MaxArmor { get; }

        public float Attack { get; }

        public float Accuracy { get; }

        public float Evasion { get; }

        public float MaxMovementDistance { get; }

        public float this[BattleStat battleStat] { get; }
    }
}
