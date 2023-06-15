﻿using CharacterAbility;
using OrderElimination.AbilitySystem;
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
            { ValueUnits.Enemies, " враг" },
        };
        private Dictionary<Buff_Type, string> _buffNames = new()
        {
            { Buff_Type.Movement, "Перемещение" },
            { Buff_Type.Attack, "Атака" },
            { Buff_Type.Health, "Здоровье" },
            { Buff_Type.Evasion, "Уклонение" },
            { Buff_Type.IncomingAccuracy, "Вх. точность" },
            { Buff_Type.IncomingDamageIncrease, "Вх. урон" },
            { Buff_Type.OutcomingAccuracy, "Вых. точность" },
            { Buff_Type.OutcomingAttack, "Вых. урон" },
            { Buff_Type.Accuracy, "Точность" },
            { Buff_Type.Concealment, "Скрытность" },
            { Buff_Type.AdditionalArmor, "Доп. броня" },
            { Buff_Type.Stun, "Оглушение" },
        };
        private Dictionary<OverTimeAbilityType, string> _overtimeTypeNames = new()
        {
            { OverTimeAbilityType.Damage, "Ур/ход" },
            { OverTimeAbilityType.Heal, "Леч/ход" },
        };

        public string GetWindowTitleName(PanelType windowType) => _panelTitleNames[windowType];
        //public string GetEffectParameterName( windowType) => _panelTitleNames[windowType];
        public string GetUnits(ValueUnits unitType) => _unitNames[unitType];
        public string GetBuffName(Buff_Type buffType) => _buffNames[buffType];
        public string GetOvertimeTypeName(OverTimeAbilityType overtimeType) => _overtimeTypeNames[overtimeType];
        public string GetBattleStatName(BattleStat battleStat)
        {
            return battleStat switch
            {
                BattleStat.MaxHealth => "Макс. здоровье",
                BattleStat.MaxArmor => "Макс. броня",
                BattleStat.AttackDamage => "Урон",
                BattleStat.Accuracy => "Точность",
                BattleStat.Evasion => "Уклонение",
                BattleStat.MaxMovementDistance => "Дальность перемещения",
                _ => throw new System.NotImplementedException(),
            };
        }
    }
}