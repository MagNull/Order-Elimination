using CharacterAbility;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement.trashToRemove_Mockups
{
    public class ExplorationResult
    {
        public int PrimaryCurrencyRecieved = Random.Range(0, 500);
        public int SpecialCurrencyRecieved = Random.Range(0, 20);
        public int ExperienceAmount = Random.Range(0, 200);
        public List<Character> SquadCharacters = GetSquadCharacters();
        public List<Powerup> PowerupsRecieved = GetPowerups();

        private static List<Character> GetSquadCharacters()
        {
            var characters = new Character[Random.Range(1, 5)];
            for (var i = 0; i < characters.Length; i++)
            {
                characters[i] = new Character();
            }
            return characters.ToList();
        }

        private static List<Powerup> GetPowerups()
        {
            var powerups = new Powerup[Random.Range(1, 5)];
            for (var i = 0; i < powerups.Length; i++)
            {
                powerups[i] = new Powerup();
            }
            return powerups.ToList();
        }
    }

    public class BattleResult
    {
        public int PrimaryCurrencyRecieved = Random.Range(0, 500);
        public int SpecialCurrencyRecieved = Random.Range(0, 20);
        public int ExperienceAmount = Random.Range(0, 200);
        public List<Character> SquadCharacters = GetSquadCharacters();
        public List<Powerup> PowerupsRecieved = GetPowerups();

        private static List<Character> GetSquadCharacters()
        {
            var characters = new Character[Random.Range(1, 5)];
            for (var i = 0; i < characters.Length; i++)
            {
                characters[i] = new Character();
            }
            return characters.ToList();
        }

        private static List<Powerup> GetPowerups()
        {
            var powerups = new Powerup[Random.Range(1, 20)];
            for (var i = 0; i < powerups.Length; i++)
            {
                powerups[i] = new Powerup();
            }
            return powerups.ToList();
        }
    }

    public class Powerup : ScriptableObject
    {
        public Sprite Icon;
    }

    #region Character
    public class BattleStats
    {
        public float HP { get; } = Random.Range(10, 800);
        public float Attack { get; } = Random.Range(10, 800);
        public float Armor { get; } = Random.Range(10, 800);
        public float Evasion { get; } = Random.Range(10, 100);
        public float Accuracy { get; } = Random.Range(10, 100);
        public float Movement { get; } = Random.Range(1, 5);
    }

    public class StrategyStats
    {
        public float BuyPrice { get; } = Random.Range(10, 800);
        public float MaintenanceCost { get; } = Random.Range(10, 800);
    }

    public class Character
    {
        public string Name { get; } = $"Soldier {Random.Range(0, 76)}";
        public bool IsDead = Random.Range(0, 100) < 20;
        private readonly BattleStats battleStats = new BattleStats();
        private readonly StrategyStats strategyStats = new StrategyStats();
        public BattleStats GetBattleStats() => battleStats;
        public StrategyStats GetStrategyStats() => strategyStats;
    }
    #endregion Character

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
                { Buff_Type.IncomingAccuracy, "Входящая точность" },
                { Buff_Type.IncomingDamageIncrease, "Входящий урон" },
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

    public class PlanetPointOrderInfo
    {
        public class OrderInfoParameter
        {
            public readonly string Name;
            public readonly string Description;
            public float Value;
            [SerializeField] private readonly Sprite _icon;

            public OrderInfoParameter(string name, float value, string description = "")
            {
                Name = name; Value = value; Description = description;
            }

            public override string ToString()
                => $"{Name}: {Value}";
        }

        public List<OrderInfoParameter> ExplorationParameters
            = new List<OrderInfoParameter>()
            {
                new OrderInfoParameter("Предметы", 20, "Шанс находки предметов"),
                new OrderInfoParameter("Опыт", 30, "Процент получаемого опыта"),
                new OrderInfoParameter("Шанс отпора", 10, "Вероятность начала боя"),
            };

        public float ExploreItemsFindChance = 20;
        public float ExploreExperiencePercent = 30;
        public float ExploreAmbushChance = 10;
        public float BattleItemsFindChance = 30;
        public float BattleExperiencePercent = 100;
        public int BattleEnemyLevel = Random.Range(1, 8);
    }
}
