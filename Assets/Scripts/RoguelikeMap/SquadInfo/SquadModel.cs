using System;
using System.Collections.Generic;
using System.Linq;
using OrderElimination.AbilitySystem;
using OrderElimination.MetaGame;
using RoguelikeMap.Panels;
using RoguelikeMap.Points;
using RoguelikeMap.UI.Characters;
using UnityEngine;

namespace OrderElimination
{
    public class SquadModel
    {
        private List<GameCharacter> _members;
        private SquadMembersPanel _panel;
        private int _activeMembersCount = 3;
        
        public IReadOnlyList<GameCharacter> ActiveMembers =>
            _members.GetRange(0, _activeMembersCount);
        public IReadOnlyList<GameCharacter> InactiveMembers => 
            _members.GetRange(_activeMembersCount, _members.Count - _activeMembersCount);
        public PointModel Point { get; private set; }
        public int AmountOfMembers => _members.Count;
        public IReadOnlyList<GameCharacter> Members => _members;

        public event Action OnUpdateSquadMembers;
        
        public SquadModel(IEnumerable<GameCharacter> members, SquadMembersPanel squadMembersPanel)
        {
            var characters = members.ToList();
            if (characters.Count == 0)
                return;
            //First three members are active
            SetSquadMembers(characters, _activeMembersCount);

            //TODO: Restore upgrades
            //UpgradeCharacters();
            SetPanel(squadMembersPanel);
        }
        
        private void SetPanel(SquadMembersPanel panel)
        {
            _panel = panel;
            panel.UpdateMembers(ActiveMembers, InactiveMembers);
        }

        public void Add(GameCharacter member) => _members.Add(member);

        public void RemoveCharacter(GameCharacter member)
        {
            if (!_members.Contains(member))
                throw new ArgumentException("No such character in squad");
            _members.Remove(member);
        }
        
        private void UpgradeCharacters()
        {
            var stats = SquadMediator.Stats.Value;
            var statsGrowth = new Dictionary<BattleStat, float>()
            {
                { BattleStat.MaxHealth, stats.HealthGrowth },
                { BattleStat.MaxArmor, stats.ArmorGrowth },
                { BattleStat.AttackDamage, stats.AttackGrowth },
                { BattleStat.Accuracy, stats.AccuracyGrowth },
                { BattleStat.Evasion, stats.EvasionGrowth },
            };

            foreach (var member in _members)
            {
                foreach (var stat in statsGrowth.Keys)
                    member.ChangeStat(stat, member.CharacterStats[stat] + statsGrowth[stat]);
            }
        }

        public void DistributeExperience(float expirience)
        {
            throw new NotImplementedException();
            foreach (var member in _members)
            {
                //member.RaiseExperience(expirience / AmountOfMembers);
            }
        }
        
        public void HealCharacters(int amountHeal)
        {
            throw new NotImplementedException();
            foreach (var member in _members)
            {
                //member.Heal(amountHeal);
            }
        }
        
        public void SetPoint(PointModel point)
        {
            Point = point;
        }

        public void SetSquadMembers(IEnumerable<GameCharacter> characters, int activeMembersCount)
        {
            _members = characters.ToList();
            _activeMembersCount = activeMembersCount;
            Debug.Log(activeMembersCount);
            OnUpdateSquadMembers?.Invoke();
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