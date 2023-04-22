using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem.OuterComponents
{
    [Serializable]
    public struct ReadOnlyBaseStats
    {
        public float MaxHealth;
        public float MaxArmor;
        public float AttackDamage;
        public float Accuracy;
        public float Evasion;
        public float MaxMovementDistance;

        public ReadOnlyBaseStats(float maxHealth, float maxArmor, float attack, float accuracy, float evasion, float movement)
        {
            MaxHealth = maxHealth;
            MaxArmor = maxArmor;
            AttackDamage = attack;
            Accuracy = accuracy;
            Evasion = evasion;
            MaxMovementDistance = movement;
        }
    }
}
