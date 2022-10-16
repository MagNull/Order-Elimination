using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UIManagement.trashToRemove_Mockups;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement.Elements
{
    public class CharacterListElement: MonoBehaviour
    {
        public string CharacterName
        {
            get => _characterName.text;
            set => _characterName.text = value;
        }

        public string MaintenanceText
        {
            get => _maintenancePriceText.text;
            set => _maintenancePriceText.text = value;
        }

        public string MaintenancePrice
        {
            get => _maintenancePriceValue.text;
            set => _maintenancePriceValue.text = value;
        }

        [SerializeField] private RectTransform _statsLayoutHolder;
        [SerializeField] private TextMeshProUGUI _characterName;
        [SerializeField] private TextMeshProUGUI _maintenancePriceText;
        [SerializeField] private TextMeshProUGUI _maintenancePriceValue;
        //[SerializeField] private IconTextValueList _stats;
        private readonly List<IconTextValueElement> _stats = new List<IconTextValueElement>();

        private void Awake()
        {

        }

        private void Start()
        {
            Setup(new Character());
        }

        private void Setup(Character character)
        {
            var battleStats = character.GetBattleStats();
            var strategyStats = character.GetStrategyStats();
            CharacterName = character.Name;
            _maintenancePriceText.text = "Содержание бойца";
            MaintenancePrice = strategyStats.MaintenanceCost.ToString();

            _stats.Add(UICommonElementsBuilder.CreateIconTextValueElement(null, "Здоровье", battleStats.HP.ToString(), _statsLayoutHolder));
            _stats.Add(UICommonElementsBuilder.CreateIconTextValueElement(null, "Урон", battleStats.Attack.ToString(), _statsLayoutHolder));
            _stats.Add(UICommonElementsBuilder.CreateIconTextValueElement(null, "Броня", battleStats.Armor.ToString(), _statsLayoutHolder));
            _stats.Add(UICommonElementsBuilder.CreateIconTextValueElement(null, "Уклонение", battleStats.Evasion.ToString(), _statsLayoutHolder));
            _stats.Add(UICommonElementsBuilder.CreateIconTextValueElement(null, "Точность", battleStats.Accuracy.ToString(), _statsLayoutHolder));
        }
    }
}
