using AI.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public readonly struct DistributedDamage
    {
        public DamageInfo IncomingDamage { get; }
        public float DamageToHealth { get; }
        public float DamageToArmor { get; }
        public float TotalDamage => DamageToHealth + DamageToArmor;

        public DistributedDamage(
            DamageInfo incomingDamage, float totalArmorDamage, float totalHealthDamage)
        {
            IncomingDamage = incomingDamage;
            DamageToHealth = totalHealthDamage;
            DamageToArmor = totalArmorDamage;
        }
    }
}
