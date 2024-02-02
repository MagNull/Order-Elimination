using System;

namespace OrderElimination.AbilitySystem
{
    public static class BattleStatHelpers
    {
        public static bool IsAbsoluteStat(this BattleStat stat)
        {
            return stat switch
            {
                BattleStat.MaxHealth => true,
                BattleStat.MaxArmor => true,
                BattleStat.AttackDamage => true,
                BattleStat.Accuracy => false,
                BattleStat.Evasion => false,
                BattleStat.MaxMovementDistance => true,
                _ => throw new NotImplementedException(),
            };
        }

        public static bool IsPercentStat(this BattleStat stat)
        {
            return stat switch
            {
                BattleStat.MaxHealth => false,
                BattleStat.MaxArmor => false,
                BattleStat.AttackDamage => false,
                BattleStat.Accuracy => true,
                BattleStat.Evasion => true,
                BattleStat.MaxMovementDistance => false,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
