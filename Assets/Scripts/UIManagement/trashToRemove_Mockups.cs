using CharacterAbility;
using OrderElimination;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement.trashToRemove_Mockups
{
    public class CharacterUpgradeTransaction
    {
        public Character TargetCharacter { get; private set; }
        public int Cost => TargetCharacter.GetStrategyStats().CostOfUpgrade;

        public const int MaximumLevelCap = 10;

        private Information _playerInformation;

        public CharacterUpgradeTransaction(Character target, Information playerInformation)
        {
            TargetCharacter = target;
            _playerInformation = playerInformation;
        }

        public bool TryUpgrade()
        {
            var availableMoney = _playerInformation.Money;
            if (Cost > availableMoney)
                return false;
            TargetCharacter.Upgrade();
            availableMoney -= Cost;
            _playerInformation.SetMoney(availableMoney);
            return true;
        }
    }

    public class BattleResult
    {
        public readonly BattleOutcome Outcome;
        public readonly Character[] SquadCharacters;
        public readonly int PrimaryCurrencyReceived;
        public readonly int SpecialCurrencyReceived;

        public BattleResult(BattleOutcome outcome, Character[] squadCharacters, int primaryCurrencyReceived, int specialCurrencyReceived)
        {
            Outcome = outcome;
            SquadCharacters = squadCharacters;
            PrimaryCurrencyReceived = primaryCurrencyReceived;
            SpecialCurrencyReceived = specialCurrencyReceived;
        }
    }

    public class Localization
    {
        public static Localization Current = new Localization();

        private Dictionary<PanelType, string> _panelTitleNames = new Dictionary<PanelType, string>()
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
        };
        private Dictionary<ValueUnits, string> _unitNames
            = new Dictionary<ValueUnits, string>()
        {
                { ValueUnits.None, "" },
                { ValueUnits.Percents, "%" },
                { ValueUnits.Cells, " клет." },
                { ValueUnits.Turns, " ход." },
        };
        private Dictionary<Buff_Type, string> _buffNames
            = new Dictionary<Buff_Type, string>()
        {
                { Buff_Type.Movement, "Перемещение" },
                { Buff_Type.Attack, "Атака" },
                { Buff_Type.Health, "Здоровье" },
                { Buff_Type.Evasion, "Уклонение" },
                { Buff_Type.IncomingAccuracy, "Вход. точность" },
                { Buff_Type.IncomingDamageIncrease, "Вход. урон" },
                { Buff_Type.Accuracy, "Точность" },
                { Buff_Type.AdditionalArmor, "Доп. броня" },
        };
        private Dictionary<OverTimeAbilityType, string> _overtimeTypeNames
            = new Dictionary<OverTimeAbilityType, string>()
        {
                { OverTimeAbilityType.Damage, "Ур/ход" },
                { OverTimeAbilityType.Heal, "Леч/ход" },
        };

        public string GetWindowTitleName(PanelType windowType) => _panelTitleNames[windowType];
        //public string GetEffectParameterName( windowType) => _panelTitleNames[windowType];
        public string GetUnits(ValueUnits unitType) => _unitNames[unitType];
        public string GetBuffName(Buff_Type buffType) => _buffNames[buffType];
        public string GetOvertimeTypeName(OverTimeAbilityType overtimeType) => _overtimeTypeNames[overtimeType];
    }

    public class MasterVolume
    {
        public static float SoundVolume { get; set; }
        public static float MusicVolume { get; set; }

        //public static void SetSoundVolume(float fractuteValue) { }
        //public static void SetMusicVolume(float fractuteValue) { }
    }
}