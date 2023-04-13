using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace OrderElimination.AbilitySystem
{
    public enum BattleStat
    {
        MaxHealth,
        MaxArmor,
        AttackDamage,
        Accuracy,
        Evasion,
        MaxMovementDistance
    }

    //public interface IBattleStats
    //{
    //    public bool HasParameter(BattleStat battleStat);

    //    public ProcessingParameter<float> this[BattleStat battleStat] => GetParameter(battleStat);
    //    public ProcessingParameter<float> GetParameter(BattleStat battleStat);
    //}

    public interface IBattleStats : ILifeStats//ICharacterBattleStats : IBattleStats
    {
        public ProcessingParameter<float> AttackDamage { get; }
        public ProcessingParameter<float> Accuracy { get; }
        public ProcessingParameter<float> Evasion { get; }
        public ProcessingParameter<float> MaxMovementDistance { get; }

        public bool HasParameter(BattleStat battleStat);

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

        //Можно заменить изменением ценой способности перемещения у конкретного персонажа. Тогда очки должны быть дробными (энергия).
        //ActionPoint.MovementPoint: float
        //public int MovementPointsPerMove { get; } //Очки перемещения, начислясляющиеся каждый ход
    }

    public interface ILifeStats
    {
        public float Health { get; set; }
        public ProcessingParameter<float> MaxHealth { get; }
        public float TotalArmor { get; set; }
        public float PureArmor { get; set; } //Value between [0 and MaxArmor]
        public float TemporaryArmor { get; } //Depletes first. Truncates at 0.
        public void AddTemporaryArmor(TemporaryArmor armor);
        public void RemoveTemporaryArmor(TemporaryArmor armor);
        public ProcessingParameter<float> MaxArmor { get; } //ValueChanged += Update Armor
    }

    public class BattleStats : ILifeStats
    {
        public float Health { get; set; }
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
                    var remainder = -offset;
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

        public ProcessingParameter<float> MaxHealth { get; }
        public ProcessingParameter<float> MaxArmor { get; }

        private readonly List<TemporaryArmor> _temporaryArmors = new List<TemporaryArmor>();
        public void AddTemporaryArmor(TemporaryArmor armor) => _temporaryArmors.Add(armor);
        public void RemoveTemporaryArmor(TemporaryArmor armor) => _temporaryArmors.Remove(armor);
    }

    public class TemporaryArmor
    {
        public float Value { get; set; }
    }
}
