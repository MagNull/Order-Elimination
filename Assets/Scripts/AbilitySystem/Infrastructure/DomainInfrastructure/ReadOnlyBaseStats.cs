using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.Domain
{
    public struct ReadOnlyBaseStats
    {
        [OdinSerialize, HideInInspector]
        private float _maxHealth;
        [OdinSerialize, HideInInspector]
        private float _maxArmor;
        [OdinSerialize, HideInInspector]
        private float _attackDamage;
        [OdinSerialize, HideInInspector]
        private float _accuracy;
        [OdinSerialize, HideInInspector]
        private float _evasion;
        [OdinSerialize, HideInInspector]
        private float _maxMovementDistance;

        [ShowInInspector][PropertyOrder(0)]
        public float MaxHealth
        {
            get => _maxHealth;
            set
            {
                if (value < 0) value = 0;
                _maxHealth = value;
            }
        }

        [ShowInInspector][PropertyOrder(2)]
        public float MaxArmor
        {
            get => _maxArmor;
            set
            {
                if (value < 0) value = 0;
                _maxArmor = value;
            }
        }

        [ShowInInspector][PropertyOrder(1)]
        public float AttackDamage
        {
            get => _attackDamage;
            set
            {
                if (value < 0) value = 0;
                _attackDamage = value;
            }
        }

        [MinValue(0), MaxValue(1)]
        [ShowInInspector][PropertyOrder(4)]
        public float Accuracy
        {
            get => _accuracy;
            set
            {
                if (value < 0) value = 0;
                if (value > 1) value = 1;
                _accuracy = value;
            }
        }

        [MinValue(0), MaxValue(1)]
        [ShowInInspector][PropertyOrder(3)]
        public float Evasion
        {
            get => _evasion;
            set
            {
                if (value < 0) value = 0;
                if (value > 1) value = 1;
                _evasion = value;
            }
        }

        [ShowInInspector][PropertyOrder(5)]
        public float MaxMovementDistance
        {
            get => _maxMovementDistance;
            set
            {
                if (value < 0) value = 0;
                _maxMovementDistance = value;
            }
        }

        public ReadOnlyBaseStats(float maxHealth, float maxArmor, float attack, float accuracy, float evasion, float movement)
        {
            _maxHealth = Mathf.Max(0, maxHealth);
            _maxArmor = Mathf.Max(0, maxArmor);
            _attackDamage = Mathf.Max(0, attack);
            _accuracy = Mathf.Clamp01(accuracy);
            _evasion = Mathf.Clamp01(evasion);
            _maxMovementDistance = Mathf.Max(0, movement);
        }
    }
}
