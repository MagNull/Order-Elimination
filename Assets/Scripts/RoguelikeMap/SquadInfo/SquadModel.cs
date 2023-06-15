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
            var characters = 
                GameCharactersFactory.CreateGameEntities(members.Select(c => c.CharacterData))
                .ToList();//Grenade here
            if (characters.Count == 0)
                return;
            //First three members are active
            SetSquadMembers(characters, _activeMembersCount);

            RestoreUpgrades();
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
                Logging.LogException( new ArgumentException("No such character in squad"));
            _members.Remove(member);
        }
        
        private void RestoreUpgrades()
        {
            var stats = SquadMediator.PlayerSquadStats.Value;
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
                {
                    var originalStat = member.CharacterStats[stat];
                    float newStat = stat == BattleStat.Accuracy || stat == BattleStat.Evasion
                        ? originalStat + statsGrowth[stat] / 100
                        : Mathf.RoundToInt(originalStat + (originalStat * statsGrowth[stat] / 100));
                    //Так можно (округление), потому что перс создаётся заново
                    //По идее...
                    //(Всё равно хуйня)
                    member.ChangeStat(stat, newStat);
                    Logging.Log($"{member.CharacterData.Name}[{stat}]: {originalStat} -> {newStat}; StatGrow: {statsGrowth[stat]}");
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
            Logging.LogException( new NotImplementedException());
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