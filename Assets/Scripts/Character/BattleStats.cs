using System;
using UnityEngine;

namespace OrderElimination
{
    [Serializable]
    public struct BattleStats
    {
        [SerializeField]
        private float _health;
        [SerializeField]
        private float _attack;
        [SerializeField]
        private float _armor;
        [SerializeField]
        private float _evasion;
        [SerializeField]
        private float _accuracy;
        [SerializeField]
        private int _movement;

        public float Health => _health;

        public float Attack => _attack;

        public float Armor => _armor;

        public float Evasion => _evasion;

        public float Accuracy => _accuracy;

        public int Movement => _movement;
    }
}