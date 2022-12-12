using System;
using UnityEngine;

namespace OrderElimination
{
    [Serializable]
    public struct BattleStats : IReadOnlyBattleStats
    {
        [SerializeField]
        private int _health;
        private int _unmodifiedHealth;
        [SerializeField]
        private int _attack;
        private int _unmodifiedAttack;
        [SerializeField]
        private int _armor;
        private int _unmodifiedArmor;
        [SerializeField]
        private int _evasion;
        private int _unmodifiedEvasion;
        [SerializeField]
        private int _accuracy;
        private int _unmodifiedAccuracy;
        [SerializeField]
        private int _movement;
        [SerializeField]
        private DamageModificator _damageModificator;
        private int _unmodifiedMovement;

        public BattleStats(IReadOnlyBattleStats other)
        {
            _health = other.Health;
            _unmodifiedHealth = other.Health;
            _attack = other.Attack;
            _unmodifiedAttack = other.Attack;
            _armor = other.Armor;
            _unmodifiedArmor = other.Armor;
            _evasion = other.Evasion;
            _unmodifiedEvasion = other.Evasion;
            _accuracy = other.Accuracy;
            _unmodifiedAccuracy = other.Accuracy;
            _movement = other.Movement;
            _unmodifiedMovement = other.Movement;
            _damageModificator = other.DamageModificator;
        }

        public int Health
        {
            get => _health;
            set
            {
                if (value < 0)
                {
                    Debug.LogWarning("Try set health less than 0");
                    _health = 0;
                }
                else if (value > _unmodifiedHealth)
                {
                    Debug.LogWarning("Try set health more than unmodified health");
                    _health = _unmodifiedHealth;
                }
                else
                {
                    _health = value;
                }
            }
        }

        public int Attack
        {
            get => _attack;
            set
            {
                if (value < 0)
                {
                    Debug.LogWarning("Try set attack less than 0");
                    _attack = 0;
                }
                else
                {
                    _attack = value;
                }
            }
        }

        public int Armor
        {
            get => _armor;
            set
            {
                if (value < 0)
                {
                    Debug.LogWarning("Try set armor less than 0");
                    _armor = 0;
                }
                else
                {
                    _armor = value;
                }
            }
        }

        public int Evasion
        {
            get => _evasion;
            set
            {
                if (value < 0)
                {
                    Debug.LogWarning("Try set evasion less than 0");
                    _evasion = 0;
                }
                else if (value > 100)
                {
                    Debug.LogWarning("Try set evasion more than 100");
                    _evasion = 100;
                }
                else
                {
                    _evasion = value;
                }
            }
        }

        public int Accuracy
        {
            get => _accuracy;
            set
            {
                if (value < 0)
                {
                    Debug.LogWarning("Try set accuracy less than 0");
                    _accuracy = 0;
                }
                else if (value > 100)
                {
                    Debug.LogWarning("Try set accuracy more than 100");
                    _accuracy = 100;
                }
                else
                {
                    _accuracy = value;
                }
            }
        }

        public int Movement
        {
            get => _movement;
            set
            {
                if (value < 0)
                {
                    Debug.LogWarning("Try set movement less than 0");
                    _movement = 0;
                }
                else
                {
                    _movement = value;
                }
            }
        }

        public int UnmodifiedHealth
        {
            get => _unmodifiedHealth;
            set
            {
                if (value < 0)
                {
                    Debug.LogWarning("Try set health less than 0");
                    _unmodifiedHealth = 0;
                }
                else
                {
                    _unmodifiedHealth = value;
                }
            }
        }

        public int UnmodifiedAttack
        {
            get => _unmodifiedAttack;
            set
            {
                if (value < 0)
                {
                    Debug.LogWarning("Try set attack less than 0");
                    _unmodifiedAttack = 0;
                }
                else
                {
                    _unmodifiedAttack = value;
                }
            }
        }

        public int UnmodifiedArmor
        {
            get => _unmodifiedArmor;
            set
            {
                if (value < 0)
                {
                    Debug.LogWarning("Try set armor less than 0");
                    _unmodifiedArmor = 0;
                }
                else
                {
                    _unmodifiedArmor = value;
                }
            }
        }

        public int UnmodifiedEvasion
        {
            get => _unmodifiedEvasion;
            set
            {
                if (value < 0)
                {
                    Debug.LogWarning("Try set evasion less than 0");
                    _unmodifiedEvasion = 0;
                }
                else if (value > 100)
                {
                    Debug.LogWarning("Try set evasion more than 100");
                    _unmodifiedEvasion = 100;
                }
                else
                {
                    _unmodifiedEvasion = value;
                }
            }
        }

        public int UnmodifiedAccuracy
        {
            get => _unmodifiedAccuracy;
            set
            {
                if (value < 0)
                {
                    Debug.LogWarning("Try set accuracy less than 0");
                    _unmodifiedAccuracy = 0;
                }
                else if (value > 100)
                {
                    Debug.LogWarning("Try set accuracy more than 100");
                    _unmodifiedAccuracy = 100;
                }
                else
                {
                    _unmodifiedAccuracy = value;
                }
            }
        }

        public int UnmodifiedMovement
        {
            get => _unmodifiedMovement;
            set
            {
                if (value < 0)
                {
                    Debug.LogWarning("Try set movement less than 0");
                    _unmodifiedMovement = 0;
                }
                else
                {
                    _unmodifiedMovement = value;
                }
            }
        }

        public DamageModificator DamageModificator { get => _damageModificator; set => _damageModificator = value; }
    }

    public interface IReadOnlyBattleStats
    {
        int Health { get; }
        int UnmodifiedHealth { get; }
        int Attack { get; }
        int UnmodifiedAttack { get; }
        int Armor { get; }
        int UnmodifiedArmor { get; }
        int Evasion { get; }
        int UnmodifiedEvasion { get; }
        int Accuracy { get; }
        int UnmodifiedAccuracy { get; }
        int Movement { get; }
        int UnmodifiedMovement { get; }

        public DamageModificator DamageModificator { get; }
    }
}