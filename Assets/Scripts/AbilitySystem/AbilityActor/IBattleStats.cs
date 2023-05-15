using OrderElimination.Infrastructure;
using System;

namespace OrderElimination.AbilitySystem
{
    public interface IBattleStats
    {
        public event Action<BattleStat> StatsChanged;
        public bool HasParameter(BattleStat battleStat);
        public ProcessingParameter<float> this[BattleStat battleStat] => GetParameter(battleStat);
        public ProcessingParameter<float> GetParameter(BattleStat battleStat);
    }

    public enum BattleStat
    {
        MaxHealth,
        MaxArmor,
        AttackDamage,
        Accuracy,
        Evasion,
        MaxMovementDistance
    }

    public interface ILifeBattleStats
    {
        public ProcessingParameter<float> MaxHealth { get; }
        public ProcessingParameter<float> MaxArmor { get; } //ValueChanged += Update Armor
        public float Health { get; set; }
        public float TotalArmor { get; set; }
        public float PureArmor { get; set; } //Value between [0 and MaxArmor]
        public float TemporaryArmor { get; } //Depletes first. Truncates at 0.
        public void AddTemporaryArmor(TemporaryArmor armor);
        public void RemoveTemporaryArmor(TemporaryArmor armor);

        public event Action<ILifeBattleStats> HealthDepleted;
    }

    public enum LifeStatPriority
    {
        ArmorFirst,
        HealthFirst,
        ArmorOnly,
        HealthOnly
    }

    public class TemporaryArmor
    {
        public float Value { get; set; }
    }
}
