using System;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination
{
    public class SquadModel
    {
        private readonly List<Character> _members;
        private int _rang;
        public int AmountOfMembers => _members.Count;
        public IReadOnlyList<Character> Members => _members;

        public event Action<Vector3> Moved;
        public event Action Selected;
        public event Action Unselected;

        public SquadModel(List<Character> members)
        {
            if (members.Count == 0)
                return;
            _members = members;
            _rang = 0;
            foreach (var character in _members)
            {
                _rang += 1;
            }

            _rang /= AmountOfMembers;
        }

        public void Add(Character member) => _members.Add(member);

        public void RemoveCharacter(Character member)
        {
            if (!_members.Contains(member))
                throw new ArgumentException("No such character in squad");
            _members.Remove(member);
        }

        public void DistributeExpirience(float expirience)
        {
            foreach (var character in _members)
            {
                character.RaiseExperience(expirience / AmountOfMembers);
            }
        }

        public void Move(Point point)
        {
            Move(point.transform.position);
        }

        public void Move(Vector3 position)
        {
            Moved?.Invoke(position);
        }

        public void Select() => Selected?.Invoke();

        public void Unselect() => Unselected?.Invoke();
    }
}