using System;
using System.Collections.Generic;
using RoguelikeMap.Panels;
using RoguelikeMap.Points;
using RoguelikeMap.UI.Characters;

namespace OrderElimination
{
    public class SquadModel
    {
        private List<Character> _members;
        private SquadMembersPanel _panel;
        
        public PointModel Point { get; private set; }
        public int AmountOfMembers => _members.Count;
        public IReadOnlyList<Character> Members => _members;
        
        public SquadModel(List<Character> members, SquadMembersPanel squadMembersPanel)
        {
            if (members.Count == 0)
                return;
            _members = members;
            
            UpgradeCharacters();
            SetPanel(squadMembersPanel);
        }
        
        private void SetPanel(SquadMembersPanel panel)
        {
            _panel = panel;
            panel.UpdateMembers(_members);
        }

        public void Add(Character member) => _members.Add(member);

        public void RemoveCharacter(Character member)
        {
            if (!_members.Contains(member))
                throw new ArgumentException("No such character in squad");
            _members.Remove(member);
        }
        
        private void UpgradeCharacters()
        {
            foreach (var member in _members)
            {
                member.Upgrade(SquadMediator.Stats);
            }
        }

        public void DistributeExperience(float expirience)
        {
            foreach (var member in _members)
            {
                member.RaiseExperience(expirience / AmountOfMembers);
            }
        }
        
        public void HealCharacters(int amountHeal)
        {
            foreach (var member in _members)
            {
                member.Heal(amountHeal);
            }
        }
        
        public void SetPoint(PointModel point)
        {
            Point = point;
        }

        public void SetSquadMembers(List<Character> characters)
        {
            _members = characters;
        }

        public void SetActivePanel(bool isActive)
        {
            if (isActive)
                _panel.Open();
            else
                _panel.Close();
        }
    }
}