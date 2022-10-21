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
            get => _maintenanceCost.Text;
            set => _maintenanceCost.Text = value;
        }

        public string MaintenanceCost
        {
            get => _maintenanceCost.Value;
            set => _maintenanceCost.Value = value;
        }

        [SerializeField] private TextMeshProUGUI _characterName;
        [SerializeField] private IconTextValueElement _maintenanceCost;
        [SerializeField] private IconTextValueList _parametersList;
        public Character CharacterInfo { get; private set; }

        public bool HasMaintenanceCost
        {
            get => _maintenanceCost.gameObject.activeSelf;
            set => _maintenanceCost.gameObject.SetActive(value);
        }

        public bool HasParameters
        {
            get => _parametersList.gameObject.activeSelf;
            set => _parametersList.gameObject.SetActive(value);
        }

        public event Action<CharacterListElement> Destroyed;

        public void UpdateCharacterInfo(Character character)
        {
            if (character == null)
                throw new InvalidOperationException();
            CharacterInfo = character;
            var battleStats = CharacterInfo.GetBattleStats();
            var strategyStats = CharacterInfo.GetStrategyStats();
            CharacterName = CharacterInfo.Name;
            MaintenanceText = "Содержание бойца";
            MaintenanceCost = strategyStats.MaintenanceCost.ToString();
            _parametersList.Add(null, "Здоровье", battleStats.HP.ToString());
            _parametersList.Add(null, "Урон", battleStats.Attack.ToString());
            _parametersList.Add(null, "Броня", battleStats.Armor.ToString());
            _parametersList.Add(null, "Уклонение", battleStats.Evasion.ToString());
            _parametersList.Add(null, "Точность", battleStats.Accuracy.ToString());
        }

        private void OnDestroy()
        {
            Destroyed?.Invoke(this);
        }
    }
}
