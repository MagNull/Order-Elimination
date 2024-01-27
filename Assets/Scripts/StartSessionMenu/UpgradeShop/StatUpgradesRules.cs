using OrderElimination;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;

namespace StartSessionMenu
{
    public class StatUpgradesRules
    {
        private Dictionary<BattleStat, int> _costGrowth = new();
        private Dictionary<BattleStat, BinaryMathOperation> _upgradeOperations = new();
        private Dictionary<BattleStat, float> _increasePerLevel = new();

        public StatUpgradesRules()
        {
            _costGrowth = new()
            {
                { BattleStat.MaxHealth, 200 },
                { BattleStat.AttackDamage, 200 },
                { BattleStat.MaxArmor, 200 },
                { BattleStat.Evasion, 200 },
                { BattleStat.Accuracy, 200 },
            };
            _upgradeOperations = new()
            {
                { BattleStat.MaxHealth, BinaryMathOperation.Multiply },
                { BattleStat.AttackDamage, BinaryMathOperation.Multiply },
                { BattleStat.MaxArmor, BinaryMathOperation.Multiply },
                { BattleStat.Evasion, BinaryMathOperation.Add },
                { BattleStat.Accuracy, BinaryMathOperation.Add },
            };
            _increasePerLevel = new()
            {
                { BattleStat.MaxHealth, 0.05f },
                { BattleStat.AttackDamage, 0.05f },
                { BattleStat.MaxArmor, 0.05f },
                { BattleStat.Evasion, 0.05f },
                { BattleStat.Accuracy, 0.05f },
            };
        }

        public float MaxUpgradeLevel { get; } = 6;

        public float GetUpgradeToLevelCost(BattleStat stat, float upgradeLevel)
        {
            if (upgradeLevel < 0)
                throw new ArgumentOutOfRangeException(nameof(upgradeLevel));
            if (upgradeLevel == 0) return 0;
            if (!_costGrowth.ContainsKey(stat)) return 0;
            return _costGrowth[stat] * upgradeLevel;
        }

        public ValueModifier GetStatModifier(BattleStat stat, float upgradeLevel)
        {
            if (upgradeLevel < 0)
                throw new ArgumentOutOfRangeException(nameof(upgradeLevel));
            var operation = _upgradeOperations[stat];
            var perLevelValue = _increasePerLevel[stat];
            if (operation == BinaryMathOperation.Add || operation == BinaryMathOperation.Subtract)
                return new ValueModifier(operation, perLevelValue * upgradeLevel);
            if (operation == BinaryMathOperation.Multiply)
                return new ValueModifier(operation, perLevelValue * upgradeLevel + 1);
            throw new NotImplementedException();
        }

        public float GetEstimatedUpgradeLevel(BattleStat stat, ValueModifier modifier)
        {
            var operation = _upgradeOperations[stat];
            var perLevelValue = _increasePerLevel[stat];
            if (modifier.Operation != operation)
                throw new NotSupportedException($"Modifier for {stat} has different operation");
            if (operation == BinaryMathOperation.Add || operation == BinaryMathOperation.Subtract)
                return modifier.Operand / perLevelValue;
            if (operation == BinaryMathOperation.Multiply)
                return (modifier.Operand - 1) / perLevelValue;
            throw new NotImplementedException();
        }
    }
}
