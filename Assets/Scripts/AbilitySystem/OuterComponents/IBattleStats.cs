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
        Health,
        MaxHealth,
        Armor,
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

    public interface IBattleStats //ICharacterBattleStats : IBattleStats
    {
        public ProcessingParameter<float> Health { get; }
        public ProcessingParameter<float> MaxHealth { get; }
        public ProcessingParameter<float> Armor { get; }
        public ProcessingParameter<float> MaxArmor { get; }
        //Очередь - это только порядок добавления брони. Исчезать она может в другом.
        //Нужно как-то идентифицировать каждый слой брони. Id при добавлении не подойдёт - список меняется.
        public Queue<float> TemporaryArmorLayers { get; }
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
                BattleStat.Health => Health,
                BattleStat.MaxHealth => MaxHealth,
                BattleStat.Armor => Armor,
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

    
}
