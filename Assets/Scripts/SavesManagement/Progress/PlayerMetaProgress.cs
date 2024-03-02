using OrderElimination.MacroGame;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.SavesManagement
{
    public class PlayerMetaProgress
    {
        [PropertyTooltip("Активированные модификаторы характеристик персонажа")]
        [HideInInspector]
        [OdinSerialize]
        public StatModifiers StatUpgrades { get; set; } = new();

        [PropertyTooltip("Кол-во валюты мета-игры")]
        [MinValue(0)]
        [OdinSerialize]
        public int MetaCurrency { get; set; }

        [PropertyTooltip("Кол-во валюты в начале забега")]
        [MinValue(0)]
        [OdinSerialize]
        public int RunStartCurrency { get; set; }

        [PropertyTooltip("Максимальное кол-во персонажей, которых игрок может взять в бой")]
        [DisableIf("@true")]
        [MinValue(1)]
        [OdinSerialize]
        public int MaxSquadSize { get; set; } = 3;

        [PropertyTooltip("Размер инвентаря в начале забега")]
        [DisableIf("@true")]
        [MinValue(0)]
        [OdinSerialize]
        public int RunInventorySize { get; set; } = 100;
        //Characters inventory size / unlocked slots

        [PropertyTooltip("Лимит валюты найма персонажей для забега")]
        [MinValue(0)]
        [OdinSerialize]
        public int HireCurrencyLimit { get; set; }

        [PropertyTooltip("Персонажи, доступные для найма")]
        [OdinSerialize]
        public List<IGameCharacterTemplate> UnlockedCharacters { get; set; } = new();
        //int MaxMembersToBuyAtTheStart
        //public HashSet<ItemData> KnownItems
    }
}
