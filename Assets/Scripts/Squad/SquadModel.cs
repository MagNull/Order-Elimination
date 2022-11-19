﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination
{
    public class SquadModel : ISelectable, IMovable
    {
        private readonly List<ISquadMember> _members;
        private int _rang;
        public int AmountOfMembers => _members.Count;
        public IReadOnlyList<ISquadMember> Members => _members;

        public event Action<PlanetPoint> Moved;
        public event Action Selected;
        public event Action Unselected;

        public SquadModel(List<ISquadMember> members)
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

        public void Add(ISquadMember member) => _members.Add(member);

        public void RemoveCharacter(ISquadMember member)
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

        public void Move(PlanetPoint planetPoint)
        {
            Moved?.Invoke(planetPoint);
        }

        public void Select() => Selected?.Invoke();

        public void Unselect() => Unselected?.Invoke();
    }
}