using System.Collections.Generic;
using UnityEngine;
using System;

namespace OrderElimination
{
    public class UnitModel : ISelectable, IMovable
    {
        private Vector2Int _position;
        private readonly List<Ability> _abilites;
        public int rang { get; private set; } // зависит от характеристик персонажа
        private float _expirience;
        private float _health;
        private float _damage;
        private float _armor;
        private float _accuracy;

        public IReadOnlyList<Ability> Abilites => _abilites;
        public event Action<Vector2Int> Moved;
        public event Action Selected;
        public event Action Unselected;

        public UnitModel()
        {
            _abilites = new List<Ability>();
            _position = Vector2Int.zero;
        }

        public void Move(Vector2Int position)
        {
           _position = position;
           Moved?.Invoke(position);
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
    }
}