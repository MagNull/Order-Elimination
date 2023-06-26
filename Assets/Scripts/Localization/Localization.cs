using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;
using UIManagement;

namespace OrderElimination.Localization
{
    public class Localization
    {
        private static Lazy<Localization> _lazyLocalization = new(() => new Localization());
        public static Localization Current { get; } = _lazyLocalization.Value;

        private Dictionary<PanelType, string> _panelTitleNames = new()
        { 
            {PanelType.Pause, "Пауза"}, 
            {PanelType.Order, "Выбор приказа"}, 
            {PanelType.SquadList, "Список бойцов отряда"}, 
            {PanelType.ExplorationResult, "Итоги поиска"}, 
            {PanelType.BattleVictory, "Победа"}, 
            {PanelType.BattleDefeat, "Поражение"}, 
            {PanelType.AbilityDescription, "Описание способности"}, 
            {PanelType.PassiveSkillsDescription, "Пассивные навыки"}, 
            {PanelType.CharacterDescription, "Информация о бойце"}, 
            {PanelType.EffectsDesriptionList, "Описание эффектов"}, 
            {PanelType.CharacterUpgradable, "Информация о бойце"},
            {PanelType.Shop, "Торговец"},
            {PanelType.Event, "Событие"},
            {PanelType.SquadMembers, "Выбор бойцов отряда"},
        };
        private Dictionary<ValueUnits, string> _unitNames = new()
        {
            { ValueUnits.None, "" },
            { ValueUnits.Percents, "%" },
            { ValueUnits.Cells, "" },
            { ValueUnits.Turns, " ход" },
            //{ ValueUnits.Enemies, " враг" },
        };

        public string GetWindowTitleName(PanelType windowType) => _panelTitleNames[windowType];
        //public string GetEffectParameterName( windowType) => _panelTitleNames[windowType];
        public string GetUnits(ValueUnits unitType) => _unitNames[unitType];
        public string GetBattleStatName(BattleStat battleStat)
        {
            return battleStat switch
            {
                BattleStat.MaxHealth => "Здоровье",
                BattleStat.MaxArmor => "Броня",
                BattleStat.AttackDamage => "Урон",
                BattleStat.Accuracy => "Точность",
                BattleStat.Evasion => "Уклонение",
                BattleStat.MaxMovementDistance => "Перемещение",
                _ => throw new System.NotImplementedException(),
            };
        }
    }
}