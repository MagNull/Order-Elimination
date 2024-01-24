using OrderElimination.AbilitySystem;
using OrderElimination.SavesManagement;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.MacroGame
{
    public static class RoguelikeRunStartManager//Replace with dontdestroy-object
    {
        public static void StartNewRun(IPlayerProgress progress, ScenesMediator mediator)
        {
            if (!IsProgressValid(progress))
                throw new ArgumentException(nameof(progress));
            mediator.Register("progress", progress);
            var characters = progress.CurrentRunProgress.PosessedCharacters;
            UpgradeCharacterStats(
                characters, progress.MetaProgress.StatUpgrades);
            //Load roguelike scene
        }

        private static bool IsProgressValid(IPlayerProgress progress)
        {
            if (progress == null)
                return false;
            if (progress.CurrentRunProgress == null) 
                return false;
            if (progress.CurrentRunProgress.PosessedCharacters == null)
                return false;
            return true;
        }

        private static void UpgradeCharacterStats(
            IEnumerable<GameCharacter> characters, StrategyStats upgradeStats)
        {
            var statsGrowth = new Dictionary<BattleStat, float>()
            {
                { BattleStat.MaxHealth, upgradeStats.HealthGrowth },
                { BattleStat.MaxArmor, upgradeStats.ArmorGrowth },
                { BattleStat.AttackDamage, upgradeStats.AttackGrowth },
                { BattleStat.Accuracy, upgradeStats.AccuracyGrowth },
                { BattleStat.Evasion, upgradeStats.EvasionGrowth },
            };

            foreach (var character in characters)
            {
                var baseStats = character.CharacterData.GetBaseBattleStats();
                foreach (var stat in statsGrowth.Keys)
                {
                    var originalStat = baseStats[stat];
                    var initialStat = character.CharacterStats[stat];
                    float newStat = stat == BattleStat.Accuracy || stat == BattleStat.Evasion
                        ? originalStat + statsGrowth[stat] / 100
                        : Mathf.RoundToInt(originalStat + (originalStat * statsGrowth[stat] / 100));
                    character.ChangeStat(stat, newStat);
                    if (stat == BattleStat.MaxHealth)
                    {
                        var prevHealthPercent = character.CurrentHealth / initialStat;
                        character.CurrentHealth = newStat * prevHealthPercent;
                    }
                    Logging.Log($"{character.CharacterData.Name}[{stat}]: {initialStat} -> {newStat}; StatGrow: {statsGrowth[stat]}");
                }
            }
        }
    }
}
