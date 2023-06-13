using System;
using UnityEngine;

namespace OrderElimination
{
    [Serializable]
    public struct OldBattleStats : IReadOnlyBattleStats
    {
        [SerializeField]
        private float _health;
        [SerializeField]
        private float _unmodifiedHealth;
        [SerializeField]
        private float _attack;
        [SerializeField]
        private float _unmodifiedAttack;
        [SerializeField]
        private float _armor;
        [SerializeField]
        private float _unmodifiedArmor;
        [SerializeField]
        private float _additionalArmor;
        [SerializeField]
        private float _evasion;
        [SerializeField]
        private float _unmodifiedEvasion;
        [SerializeField]
        private float _accuracy;
        [SerializeField]
        private float _unmodifiedAccuracy;
        [SerializeField]
        private float _movement;
        [SerializeField]
        private float _unmodifiedMovement;
        [SerializeField]
        private DamageModificator _damageModificator;

        [Obsolete]
        public OldBattleStats(IReadOnlyBattleStats other)
        {
            _health = other.Health;
            _unmodifiedHealth = other.UnmodifiedHealth == 0 ? other.Health : other.UnmodifiedHealth;
            _attack = other.Attack;
            _unmodifiedAttack = other.UnmodifiedAttack == 0 ? other.Attack : other.UnmodifiedAttack;
            _armor = other.Armor;
            _unmodifiedArmor = other.UnmodifiedArmor == 0 ? other.Armor : other.UnmodifiedArmor;
            _evasion = other.Evasion;
            _unmodifiedEvasion = other.UnmodifiedEvasion == 0 ? other.Evasion : other.UnmodifiedEvasion;
            _accuracy = other.Accuracy;
            _unmodifiedAccuracy = other.UnmodifiedAccuracy == 0 ? other.Accuracy : other.UnmodifiedAccuracy;
            _movement = other.Movement;
            _unmodifiedMovement = other.UnmodifiedMovement == 0 ? other.Movement : other.UnmodifiedMovement;
            _damageModificator = other.DamageModificator;
            _additionalArmor = other.AdditionalArmor;
        }

        public float Health
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

        public float Attack
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

        public float Armor
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

        public float Evasion
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

        public float Accuracy
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

        public float Movement
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

        public float UnmodifiedHealth
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

        public float UnmodifiedAttack
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

        public float UnmodifiedArmor
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

        public float UnmodifiedEvasion
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

        public float UnmodifiedAccuracy
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

        public float UnmodifiedMovement
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

        public DamageModificator DamageModificator
        {
            get => _damageModificator;
            set => _damageModificator = value;
        }

        public float AdditionalArmor
        {
            get => _additionalArmor;
            set
            {
                if (value < 0)
                {
                    Debug.LogWarning("Try set additional armor less than 0");
                    _additionalArmor = 0;
                }
                else
                {
                    _additionalArmor = value;
                }
            }
        }
    }

    [Obsolete]
    public interface IReadOnlyBattleStats
    {
        float Health { get; }
        float UnmodifiedHealth { get; }
        float Attack { get; }
        float UnmodifiedAttack { get; }
        float Armor { get; }
        float UnmodifiedArmor { get; }
        
        float AdditionalArmor { get; }
        float Evasion { get; }
        float UnmodifiedEvasion { get; }
        float Accuracy { get; }
        float UnmodifiedAccuracy { get; }
        float Movement { get; }
        float UnmodifiedMovement { get; }

        public DamageModificator DamageModificator { get; }
    }
}