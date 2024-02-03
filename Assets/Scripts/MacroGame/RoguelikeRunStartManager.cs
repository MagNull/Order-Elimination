using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using OrderElimination.SavesManagement;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.MacroGame
{
    public static class RoguelikeRunStartManager//Replace with dontdestroy-object
    {
        public static void StartRun(IPlayerProgress progress, ScenesMediator mediator)
        {
            if (progress == null)
                throw new ArgumentNullException(nameof(progress));
            if (!IsProgressValid(progress.CurrentRunProgress))
            {
                throw new ArgumentException("Run is invalid");
            }
            mediator.Register("progress", progress);
            var characters = progress.CurrentRunProgress.PosessedCharacters;
            UpgradeCharacterStats(
                characters, progress.MetaProgress.StatUpgrades);
            //Load roguelike scene
        }

        private static bool IsProgressValid(PlayerRunProgress runProgress)
        {
            if (runProgress == null) 
                return false;
            if (runProgress.PosessedCharacters == null 
                || runProgress.PosessedCharacters.Count == 0)
                return false;
            if (runProgress.PlayerInventory == null) 
                return false;
            return true;
        }

        public static PlayerRunProgress GetInitialProgress(PlayerMetaProgress metaProgress)
            => PlayerProgressExtensions.GetInitialRunProgress(metaProgress);

        private static void UpgradeCharacterStats(
            IEnumerable<GameCharacter> characters, StatModifiers upgradeStats)
        {
            var rounding = true;
            foreach (var character in characters)
            {
                var baseStats = character.CharacterData.GetBaseBattleStats();
                var currentHealthPart = 
                    character.CurrentHealth / character.CharacterStats[BattleStat.MaxHealth];
                foreach (var stat in upgradeStats.Modifiers.Keys)
                {
                    var baseStat = baseStats[stat];
                    var upgradedStat = upgradeStats.Modifiers[stat].ModifyValue(baseStat);
                    //STATS ROUNDING
                    if (rounding)
                    {
                        if (stat.IsAbsoluteStat())
                        {
                            upgradedStat = MathExtensions.Round(upgradedStat, RoundingOption.Math);
                        }
                        else if (stat.IsPercentStat())
                        {
                            upgradedStat = MathExtensions.Round(upgradedStat * 100, RoundingOption.Math) / 100f;
                        }
                    }
                    character.ChangeStat(stat, upgradedStat);
                    if (stat == BattleStat.MaxHealth)
                    {
                        character.CurrentHealth = currentHealthPart * upgradedStat;
                    }
                    Logging.Log($"{character.CharacterData.Name}[{stat}]: {baseStat} -> {upgradedStat}");
                }
            }
        }
    }
}
