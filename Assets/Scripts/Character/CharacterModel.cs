using System.Collections.Generic;
using UnityEngine;
using System;

namespace OrderElimination
{
    public class CharacterModel : ISquadMember
    {
        private Vector2Int _position;
        private readonly List<Ability> _abilites;
        private int _rang; // ������� �� ������������� ���������
        private float _expirience;
        private float _health;
        private float _damage;
        private float _armor;
        private float _accuracy;

        public IReadOnlyList<Ability> Abilites => _abilites;
        public event Action<PlanetPoint> Moved;
        public event Action Selected;
        public event Action Unselected;

        public CharacterModel()
        {
            _abilites = new List<Ability>();
            _position = Vector2Int.zero;
        }

        public void Move(PlanetPoint planetPoint)
        {
           _position = Vector2Int.FloorToInt(planetPoint.transform.position);
           Moved?.Invoke(planetPoint);
        }

        public void RaiseExpirience(float expirience)
        {
            _expirience += expirience;
        }

        public void Select()
        {
            Selected?.Invoke();
        }

        public void Unselect()
        {
            Unselected?.Invoke();
        }

        public int GetStats()
        {
            return _rang;
        }
    }
}