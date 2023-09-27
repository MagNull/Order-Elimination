using System;
using System.Collections.Generic;
using System.Linq;
using OrderElimination.AbilitySystem;
using OrderElimination.MacroGame;
using RoguelikeMap.Points.Models;
using RoguelikeMap.UI.Characters;
using UnityEngine;

namespace OrderElimination
{
    public class SquadModel
    {
        private List<GameCharacter> _members;
        private SquadMembersPanel _panel;
        private int _activeMembersCount = 3;
        private ScenesMediator _mediator;
        
        public IReadOnlyList<GameCharacter> ActiveMembers =>
            _members.GetRange(0, _activeMembersCount);
        public IReadOnlyList<GameCharacter> InactiveMembers => 
            _members.GetRange(_activeMembersCount, _members.Count - _activeMembersCount);
        public PointModel Point { get; private set; }
        public int AmountOfMembers => _members.Count;
        public IReadOnlyList<GameCharacter> Members => _members;

        public event Action OnUpdateSquadMembers;
        
        public SquadModel(IEnumerable<GameCharacter> members, SquadMembersPanel squadMembersPanel,
            ScenesMediator scenesMediator)
        {
            _mediator = scenesMediator;
            //var characters = 
            //    GameCharactersFactory.CreateGameCharacters(members.Select(c => c.CharacterData))
            //    .ToList();//Grenade here
            _activeMembersCount = members.Count();
            if (members.Count() == 0)
                throw new ArgumentException($"Attempt to create {nameof(SquadModel)} with 0 members.");
            //First three members are active
            SetSquadMembers(members, _activeMembersCount);

            RestoreUpgrades();
            SetPanel(squadMembersPanel);
        }
        
        private void SetPanel(SquadMembersPanel panel)
        {
            _panel = panel;
            panel.UpdateMembers(ActiveMembers, InactiveMembers);
        }

        public void Add(IEnumerable<GameCharacter> members)
        {
            _members.AddRange(members);
            UpdateActiveMembersCount();
            _panel.UpdateMembers(ActiveMembers, InactiveMembers);
        }

        public void RemoveCharacter(GameCharacter member)
        {
            if (!_members.Contains(member))
                Logging.LogException( new ArgumentException("No such character in squad"));
            _members.Remove(member);
            UpdateActiveMembersCount();
        }

        private void UpdateActiveMembersCount()
        {
            _activeMembersCount = _members.Count <= 3 ? _members.Count : 3;
        }
        
        private void RestoreUpgrades()
        {
            var stats = _mediator.Get<StrategyStats>("stats");
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
                var baseStats = member.CharacterData.GetBaseBattleStats();
                foreach (var stat in statsGrowth.Keys)
                {
                    var originalStat = baseStats[stat];
                    var initialStat = member.CharacterStats[stat];
                    float newStat = stat == BattleStat.Accuracy || stat == BattleStat.Evasion
                        ? originalStat + statsGrowth[stat] / 100
                        : Mathf.RoundToInt(originalStat + (originalStat * statsGrowth[stat] / 100));
                    member.ChangeStat(stat, newStat);
                    if (stat == BattleStat.MaxHealth)
                    {
                        var prevHealthPercent = member.CurrentHealth / initialStat;
                        member.CurrentHealth = newStat * prevHealthPercent;
                    }
                    Logging.Log($"{member.CharacterData.Name}[{stat}]: {initialStat} -> {newStat}; StatGrow: {statsGrowth[stat]}");
                }
            }
        }

        public void DistributeExperience(float expirience)
        {
            Logging.LogException( new NotImplementedException());
            foreach (var member in _members)
            {
                //member.RaiseExperience(expirience / AmountOfMembers);
            }
        }
        
        public void HealCharacters(int amountHeal)
        {
            foreach (var member in _members)
            {
                member.CurrentHealth += amountHeal;
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