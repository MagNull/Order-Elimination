using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination 
{
    public class SquadModel : ISelectable, IMovable
    {
        private List<Unit> _units;
        private int _rang;

        public int AmountOfUnits => _units.Count;
        public IReadOnlyList<Unit> Units => _units;

        public event Action<Vector2Int> Moved;
        public event Action Selected;
        public event Action Unselected;

        public SquadModel(List<Unit> units)
        {
            _units = units;
            _rang = 0;
            foreach (var unit in units)
            {
                _rang += unit.Rang;
            }
            _rang /= AmountOfUnits;
        }

        public void AddUnit(Unit unit)
        {
            _units.Add(unit);
        }

        public void RemoveUnit(Unit unit)
        {
            if (!_units.Contains(unit))
                throw new ArgumentException("No such unit in squad");
            _units.Remove(unit);
        }

        public void DistributeExpirience(float expirience)
        {
            foreach (var unit in _units)
            {
                unit.RaiseExpirience(expirience / AmountOfUnits);
            }
        }
        
        public void Move(Vector2Int position)
        {
            foreach(var unit in _units)
            {
                unit.Move(position);
            }
            Moved?.Invoke(position);
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
