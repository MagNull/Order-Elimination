using OrderElimination.AbilitySystem.OuterComponents;
using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace OrderElimination.AbilitySystem
{
    public interface IBattleStats
    {
        public bool HasParameter(BattleStat battleStat);

        public ProcessingParameter<float> this[BattleStat battleStat] => GetParameter(battleStat);
        public ProcessingParameter<float> GetParameter(BattleStat battleStat);
    }

    public class BattleStats : IBattleStats, IBattleLifeStats
    {
        private readonly List<TemporaryArmor> _temporaryArmors = new List<TemporaryArmor>();
        private float _health;

        public ProcessingParameter<float> AttackDamage { get; } = new ProcessingParameter<float>();
        public ProcessingParameter<float> Accuracy { get; } = new ProcessingParameter<float>();
        public ProcessingParameter<float> Evasion { get; } = new ProcessingParameter<float>();
        public ProcessingParameter<float> MaxMovementDistance { get; } = new ProcessingParameter<float>();
        public ProcessingParameter<float> MaxHealth { get; } = new ProcessingParameter<float>();
        public ProcessingParameter<float> MaxArmor { get; } = new ProcessingParameter<float>();

        public float Health
        {
            get => _health;
            set
            {
                if (value > 0)
                    _health = value;
                else
                {
                    _health = 0;
                    HealthDepleted?.Invoke(this);
                }
            }
        }
        public float TotalArmor
        {
            get => PureArmor + TemporaryArmor;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException();
                var offset = TotalArmor - value;
                if (value < TotalArmor)//dmg
                {
                    //depletes TempArmor first, then Pure
                    var remainder = offset;
                    while (remainder > 0 && _temporaryArmors.Count > 0)
                    {
                        var removedPart = MathF.Min(_temporaryArmors[0].Value, remainder);
                        _temporaryArmors[0].Value -= removedPart;
                        remainder -= removedPart;
                        if (_temporaryArmors[0].Value <= 0)
                            _temporaryArmors.RemoveAt(0);
                    }
                    PureArmor -= MathF.Min(PureArmor, remainder);
                }
                else if (value > TotalArmor)//heal
                {
                    //only heals PureArmor
                    PureArmor = MathF.Min(PureArmor + offset, MaxArmor.ModifiedValue);
                }
            }
        }
        public float PureArmor { get; set; }
        public float TemporaryArmor => _temporaryArmors.Sum(a => a.Value);

        public void AddTemporaryArmor(TemporaryArmor armor) => _temporaryArmors.Add(armor);
        public void RemoveTemporaryArmor(TemporaryArmor armor) => _temporaryArmors.Remove(armor);

        public bool HasParameter(BattleStat battleStat)
        {
            if (battleStat == BattleStat.MaxHealth
                || battleStat == BattleStat.MaxArmor
                || battleStat == BattleStat.Accuracy
                || battleStat == BattleStat.AttackDamage
                || battleStat == BattleStat.Evasion
                || battleStat == BattleStat.MaxMovementDistance)
                return true;
            return false;
        }
        public ProcessingParameter<float> this[BattleStat battleStat] => GetParameter(battleStat);
        public ProcessingParameter<float> GetParameter(BattleStat battleStat)
        {
            if (!HasParameter(battleStat)) throw new ArgumentException();
            return battleStat switch
            {
                BattleStat.MaxHealth => MaxHealth,
                BattleStat.MaxArmor => MaxArmor,
                BattleStat.AttackDamage => AttackDamage,
                BattleStat.Accuracy => Accuracy,
                BattleStat.Evasion => Evasion,
                BattleStat.MaxMovementDistance => MaxMovementDistance,
                _ => throw new ArgumentException(),
            };
        }

        public event Action<IBattleLifeStats> HealthDepleted;

        public BattleStats(ReadOnlyBaseStats baseStats)
        {
            MaxHealth.SetUnmodifiedValue(baseStats.MaxHealth);
            MaxArmor.SetUnmodifiedValue(baseStats.MaxArmor);
            AttackDamage.SetUnmodifiedValue(baseStats.AttackDamage);
            Accuracy.SetUnmodifiedValue(baseStats.Accuracy);
            Evasion.SetUnmodifiedValue(baseStats.Evasion);
            MaxMovementDistance.SetUnmodifiedValue(baseStats.MaxMovementDistance);

            Health = MaxHealth.ModifiedValue;
            PureArmor = MaxArmor.ModifiedValue;

            MaxArmor.ValueChanged += OnMaxArmorChanged;
            MaxHealth.ValueChanged += OnMaxHealthChanged;
        }
        //Можно заменить изменением ценой способности перемещения у конкретного персонажа. Тогда очки должны быть дробными (энергия).
        //ActionPoint.MovementPoint: float
        //public int MovementPointsPerMove { get; } //Очки перемещения, начислясляющиеся каждый ход

        private void OnMaxArmorChanged(ProcessingParameter<float> parameter)
        {
            var maxArmor = MaxArmor.ModifiedValue;
            if (maxArmor < PureArmor)
                PureArmor = maxArmor;
        }

        private void OnMaxHealthChanged(ProcessingParameter<float> parameter)
        {
            var maxHealth = MaxHealth.ModifiedValue;
            if (maxHealth < Health)
                Health = maxHealth;
        }
    }

    public interface IBattleLifeStats
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

    public class TemporaryArmor
    {
        public float Value { get; set; }
    }
}
