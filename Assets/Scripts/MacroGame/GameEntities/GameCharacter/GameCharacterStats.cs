using OrderElimination.AbilitySystem;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using UnityEngine;

namespace OrderElimination.MacroGame
{
    [Serializable]
    public struct GameCharacterStats : IReadOnlyGameCharacterStats
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
                if (value < 0)
                    throw new ArgumentOutOfRangeException();
                _maxHealth = value;
            }
        }

        [ShowInInspector][PropertyOrder(2)]
        public float MaxArmor
        {
            get => _maxArmor;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException();
                _maxArmor = value;
            }
        }

        [ShowInInspector][PropertyOrder(1)]
        public float Attack
        {
            get => _attackDamage;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException();
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
                //if (value < 0) value = 0;
                //if (value > 1) value = 1;
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
                //if (value < 0) value = 0;
                //if (value > 1) value = 1;
                _evasion = value;
            }
        }

        [ShowInInspector][PropertyOrder(5)]
        public float MaxMovementDistance
        {
            get => _maxMovementDistance;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException();
                _maxMovementDistance = value;
            }
        }

        public GameCharacterStats(
            float maxHealth, float maxArmor, float attack, float accuracy, float evasion, float movement)
        {
            if (maxHealth < 0
                || maxArmor < 0
                || attack < 0
                || movement < 0)
                throw new ArgumentOutOfRangeException();
            _maxHealth = maxHealth;
            _maxArmor = maxArmor;
            _attackDamage = attack;
            _accuracy = accuracy;
            _evasion = evasion;
            _maxMovementDistance = movement;
        }

        public GameCharacterStats(IReadOnlyGameCharacterStats basedStats) : this(
            basedStats.MaxHealth,
            basedStats.MaxArmor,
            basedStats.Attack,
            basedStats.Accuracy,
            basedStats.Evasion,
            basedStats.MaxMovementDistance)
        { }

        public float this[BattleStat battleStat]
        {
            get => battleStat switch
            {
                BattleStat.MaxHealth => MaxHealth,
                BattleStat.MaxArmor => MaxArmor,
                BattleStat.AttackDamage => Attack,
                BattleStat.Accuracy => Accuracy,
                BattleStat.Evasion => Evasion,
                BattleStat.MaxMovementDistance => MaxMovementDistance,
                _ => throw new NotImplementedException(),
            };
            set
            {
                switch (battleStat)
                {
                    case BattleStat.MaxHealth:
                        MaxHealth = value;
                        break;
                    case BattleStat.MaxArmor:
                        MaxArmor = value;
                        break;
                    case BattleStat.AttackDamage:
                        Attack = value;
                        break;
                    case BattleStat.Accuracy:
                        Accuracy = value;
                        break;
                    case BattleStat.Evasion:
                        Evasion = value;
                        break;
                    case BattleStat.MaxMovementDistance:
                        MaxMovementDistance = value;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}
