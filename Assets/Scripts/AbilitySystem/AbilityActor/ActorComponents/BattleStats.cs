using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.AbilitySystem
{
    public class BattleStats : IBattleStats, IBattleLifeStats
    {
        private readonly List<TemporaryArmor> _temporaryArmors = new();
        private readonly Dictionary<ProcessingParameter<float>, BattleStat> _battleStatEnums = new();
        private float _health;
        private float _pureArmor;


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
                LifeStatsChanged?.Invoke(this);
            }
        }
        public float TotalArmor
        {
            get => PureArmor + TemporaryArmor;
            set
            {
                if (value < 0) Logging.LogException( new ArgumentOutOfRangeException());
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
                LifeStatsChanged?.Invoke(this);
            }
        }
        public float PureArmor
        {
            get => _pureArmor;
            set
            {
                if (value < 0) value = 0;
                _pureArmor = value;
                LifeStatsChanged?.Invoke(this);
            }
        }
        public float TemporaryArmor => _temporaryArmors.Sum(a => a.Value);

        public void AddTemporaryArmor(TemporaryArmor armor)
        {
            _temporaryArmors.Add(armor);
            LifeStatsChanged?.Invoke(this);
        }
        public void RemoveTemporaryArmor(TemporaryArmor armor)
        {
            _temporaryArmors.Remove(armor);

            LifeStatsChanged?.Invoke(this);
        }

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
                if (!HasParameter(battleStat)) Logging.LogException( new ArgumentException());
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

        public event Action<BattleStat> StatsChanged;
        public event Action<IBattleLifeStats> HealthDepleted;
        public event Action<IBattleLifeStats> LifeStatsChanged;

        public BattleStats(
            float maxHealth,
            float maxArmor,
            float attackDamage,
            float accuracy,
            float evasion,
            float maxMovementDistance)
        {
            MaxHealth.UnmodifiedValue = maxHealth;
            MaxArmor.UnmodifiedValue = maxArmor;
            AttackDamage.UnmodifiedValue = attackDamage;
            Accuracy.UnmodifiedValue = accuracy;
            Evasion.UnmodifiedValue = evasion;
            MaxMovementDistance.SetUnmodifiedValue(maxMovementDistance);

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
            LifeStatsChanged?.Invoke(this);
        }

        private void OnMaxHealthChanged(ProcessingParameter<float> parameter)
        {
            var maxHealth = MaxHealth.ModifiedValue;
            if (maxHealth < Health)
                Health = maxHealth;
            LifeStatsChanged?.Invoke(this);
        }

        private void OnStatsChanged(ProcessingParameter<float> parameter)
        {
            if (!_battleStatEnums.ContainsKey(parameter))
                Logging.LogException( new ArgumentException());
            StatsChanged?.Invoke(_battleStatEnums[parameter]);
        }
    }
}
