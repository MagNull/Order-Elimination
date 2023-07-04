using OrderElimination.Infrastructure;
using System;

namespace OrderElimination.AbilitySystem
{
    public interface IBattleStats
    {
        public event Action<BattleStat> StatsChanged;
        public bool HasParameter(BattleStat battleStat);
        public ProcessingParameter<float> this[BattleStat battleStat] { get; }
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

    public interface IBattleLifeStats : IBattleStats
    {
        public ProcessingParameter<float> MaxHealth { get; }
        public ProcessingParameter<float> MaxArmor { get; } //ValueChanged += Update Armor
        public float Health { get; set; }
        public float TotalArmor { get; set; }
        public float PureArmor { get; set; } //Value between [0 and MaxArmor]
        public float TemporaryArmor { get; } //Depletes first. Truncates at 0.
        public void AddTemporaryArmor(TemporaryArmor armor);
        public void RemoveTemporaryArmor(TemporaryArmor armor);

        public event Action<IBattleLifeStats> HealthDepleted;
        public event Action<IBattleLifeStats> LifeStatsChanged;
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

        public TemporaryArmor(float amount)
        {
            Value = amount;
        }
    }
}
