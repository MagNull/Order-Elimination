using Sirenix.OdinInspector;
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
    [ExecuteInEditMode]
    public class CharacterListElement: SerializedMonoBehaviour
    {
        public string CharacterName
        {
            get => _characterName.text;
            set => _characterName.text = value;
        }

        public string ExperienceRecieved
        {
            get => _experienceRecieved.Value;
            set => _experienceRecieved.Value = value;
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
        [SerializeField] private Image _characterDeadImage;
        [SerializeField] private IconTextValueElement _experienceRecieved;
        [SerializeField] private IconTextValueElement _maintenanceCost;
        [SerializeField] private IconTextValueList _parametersList;
        public BattleCharacterView CharacterInfo { get; private set; }

        [ShowInInspector]
        public bool HasExperienceRecieved
        {
            get => _experienceRecieved.gameObject.activeSelf;
            set => _experienceRecieved.gameObject.SetActive(value);
        }

        [ShowInInspector]
        public bool HasMaintenanceCost
        {
            get => _maintenanceCost.gameObject.activeSelf;
            set => _maintenanceCost.gameObject.SetActive(value);
        }

        [ShowInInspector]
        public bool HasParameters
        {
            get => _parametersList.gameObject.activeSelf;
            set => _parametersList.gameObject.SetActive(value);
        }

        [ShowInInspector]
        public bool IsDead
        {
            get => _characterDeadImage == null ? false : _characterDeadImage.gameObject.activeSelf;
            set => _characterDeadImage.gameObject.SetActive(value);
        }

        public event Action<CharacterListElement> Destroyed;

        public void UpdateCharacterInfo(BattleCharacterView character)
        {
            if (character == null)
                throw new InvalidOperationException();
            CharacterInfo = character;
            //IsDead = character.IsDead;
            var battleStats = CharacterInfo.Model.Stats;//.GetBattleStats();
            //var strategyStats = CharacterInfo.GetStrategyStats();
            CharacterName = CharacterInfo.CharacterName;
            MaintenanceText = "Содержание бойца";
            //MaintenanceCost = strategyStats.MaintenanceCost.ToString();
            _parametersList.Add(null, "Здоровье", battleStats.UnmodifiedHealth.ToString());
            _parametersList.Add(null, "Урон", battleStats.UnmodifiedAttack.ToString());
            _parametersList.Add(null, "Броня", battleStats.UnmodifiedArmor.ToString());
            _parametersList.Add(null, "Уклонение", battleStats.UnmodifiedEvasion.ToString());
            _parametersList.Add(null, "Точность", battleStats.UnmodifiedAccuracy.ToString());
        }

        private void OnDestroy()
        {
            Destroyed?.Invoke(this);
        }
    }
}
