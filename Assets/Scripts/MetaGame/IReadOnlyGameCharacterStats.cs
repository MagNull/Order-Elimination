using OrderElimination.AbilitySystem;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.MetaGame
{
    public interface IReadOnlyGameCharacterStats
    {
        public float MaxHealth { get; }

        public float MaxArmor { get; }

        public float AttackDamage { get; }

        public float Accuracy { get; }

        public float Evasion { get; }

        public float MaxMovementDistance { get; }

        public float this[BattleStat battleStat] { get; }
    }
}
