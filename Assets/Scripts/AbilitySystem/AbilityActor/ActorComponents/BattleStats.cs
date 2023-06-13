using OrderElimination.Domain;
using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.AbilitySystem
{
    public class BattleStats : IBattleStats, ILifeBattleStats
    {
        private readonly List<TemporaryArmor> _temporaryArmors = new();
        private readonly Dictionary<ProcessingParameter<float>, BattleStat> _battleStatEnums = new();
        private float _health;
        private float _pureArmor;

        public event Action<BattleStat> StatsChanged;

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
        public float PureArmor
        {
            get => _pureArmor;
            set
            {
                if (value < 0) value = 0;
                _pureArmor = value;
            }
        }
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
        public ProcessingParameter<float> this[BattleStat battleStat]
        {
            get
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

        }

        public event Action<ILifeBattleStats> HealthDepleted;

        public BattleStats(BaseBattleStats baseStats)
        {
            MaxHealth.UnmodifiedValue = baseStats.MaxHealth;
            MaxArmor.UnmodifiedValue = baseStats.MaxArmor;
            AttackDamage.UnmodifiedValue = baseStats.AttackDamage;
            Accuracy.UnmodifiedValue = baseStats.Accuracy;
            Evasion.UnmodifiedValue = baseStats.Evasion;
            MaxMovementDistance.SetUnmodifiedValue(baseStats.MaxMovementDistance);

            _battleStatEnums = new()
            {
                { MaxHealth, BattleStat.MaxHealth },
                { MaxArmor, BattleStat.MaxArmor },
                { AttackDamage, BattleStat.AttackDamage },
                { Accuracy, BattleStat.Accuracy },
                { Evasion, BattleStat.Evasion },
                { MaxMovementDistance, BattleStat.MaxMovementDistance }
            };

            Health = MaxHealth.ModifiedValue;
            PureArmor = MaxArmor.ModifiedValue;

            MaxArmor.ValueChanged += OnMaxArmorChanged;
            MaxHealth.ValueChanged += OnMaxHealthChanged;

            foreach (var stat in EnumExtensions.GetValues<BattleStat>())
            {
                if (HasParameter(stat))
                {
                    this[stat].ValueChanged += OnStatsChanged;
                }
            }
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

        private void OnStatsChanged(ProcessingParameter<float> parameter)
        {
            if (!_battleStatEnums.ContainsKey(parameter))
                throw new ArgumentException();
            StatsChanged?.Invoke(_battleStatEnums[parameter]);
        }
    }
}
