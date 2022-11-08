using System;
using UnityEngine;

namespace OrderElimination
{
    [Serializable]
    public struct BattleStats : IReadOnlyBattleStats
    {
        [SerializeField]
        private int _health;
        [SerializeField]
        private int _attack;
        [SerializeField]
        private int _armor;
        [SerializeField]
        private int _evasion;
        [SerializeField]
        private int _accuracy;
        [SerializeField]
        private int _movement;
        
        public BattleStats(IReadOnlyBattleStats other)
        {
            _health = other.Health;
            _attack = other.Attack;
            _armor = other.Armor;
            _evasion = other.Evasion;
            _accuracy = other.Accuracy;
            _movement = other.Movement;
        }

        public int Health
        {
            get => _health;
            set
            {
                if (value < 0)
                {
                    Debug.Log("Try set health less than 0");
                    _health = 0;
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
                    Debug.Log("Try set attack less than 0");
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
                    Debug.Log("Try set armor less than 0");
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
                    Debug.Log("Try set evasion less than 0");
                    _evasion = 0;
                }
                else if (value > 100)
                {
                    Debug.Log("Try set evasion more than 100");
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
                    Debug.Log("Try set accuracy less than 0");
                    _accuracy = 0;
                }
                else if (value > 100)
                {
                    Debug.Log("Try set accuracy more than 100");
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
                    Debug.Log("Try set movement less than 0");
                    _movement = 0;
                }
                else
                {
                    _movement = value;
                }
            }
        }
    }
    
    public interface IReadOnlyBattleStats
    {
        int Health { get; }
        int Attack { get; }
        int Armor { get; }
        int Evasion { get; }
        int Accuracy { get; }
        int Movement { get; }
    }
}