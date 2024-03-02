using System;
using System.Collections.Generic;
using System.Linq;
using OrderElimination.MacroGame;
using RoguelikeMap.Points.Models;
using RoguelikeMap.UI.Characters;
using UnityEngine;

namespace OrderElimination
{
    public class SquadModel
    {
        private List<GameCharacter> _members;

        public IReadOnlyList<GameCharacter> Members => _members;
        public int ActiveMembersCount { get; private set; } = 0;
        public IReadOnlyList<GameCharacter> ActiveMembers =>
            _members.GetRange(0, ActiveMembersCount);
        public IReadOnlyList<GameCharacter> InactiveMembers =>
            _members.GetRange(ActiveMembersCount, _members.Count - ActiveMembersCount);

        public event Action<SquadModel> SquadUpdated;

        public SquadModel(
            List<GameCharacter> posessedPlayerCharacters)
        {
            if (posessedPlayerCharacters.Count == 0)
                Logging.LogError($"Attempt to create {nameof(SquadModel)} with 0 members.");
            _members = posessedPlayerCharacters;//reference to progress model
            ActiveMembersCount = Mathf.Min(_members.Count, 3);
        }

        public void Add(IEnumerable<GameCharacter> members)
        {
            _members.AddRange(members);
            SquadUpdated?.Invoke(this);
        }

        public void RemoveCharacter(GameCharacter member)
        {
            if (!_members.Contains(member))
                Logging.LogException(new ArgumentException("No such character in squad"));
            _members.Remove(member);
        }

        public void SetSquadMembers(IEnumerable<GameCharacter> characters, int countActiveCharacters)
        {
            _members = characters.ToList();
            ActiveMembersCount = countActiveCharacters;
            SquadUpdated?.Invoke(this);
        }
    }
}