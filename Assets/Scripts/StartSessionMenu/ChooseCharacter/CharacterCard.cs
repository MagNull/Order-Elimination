using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using RoguelikeMap;
using UIManagement;
using UIManagement.Elements;

namespace OrderElimination
{
    public class CharacterCard : MonoBehaviour
    {
        private Character _character;
        private int _cost;

        public Character Character => _character;
        public int Cost => _cost;

        public Button Button => _button;

        private HealthBar _healthBar;
        [SerializeField] 
        private Image _cardImage;
        [SerializeField]
        private Image _costImage;
        [SerializeField]
        private Text _cardCost;
        [SerializeField] 
        private Button _button;
        private HoldableButton _holdableButton;

        public bool _isSelected;

        public void InitializeCard(Character character, int cost)
        {
            _character = character;
            _cost = cost;
            _cardImage.sprite = character.GetViewAvatar();
            _healthBar = GetComponentInChildren<HealthBar>();
            _healthBar?.SetMaxHealth(character.GetBattleStats().UnmodifiedHealth);
            if(_cardCost is not null)
                _cardCost.text = cost.ToString() + "$";
            _holdableButton = GetComponent<HoldableButton>();
            if (_holdableButton)
                _holdableButton.Holded += OnHold;
            _button.onClick = new Button.ButtonClickedEvent();
        }

        public void Select()
        {
            _isSelected = !_isSelected;
            _costImage.gameObject.SetActive(!_isSelected);
        }

        private void OnHold(HoldableButton b, float t)
        {
            var charDescPanel =
                (CharacterDescriptionPanel) UIController.SceneInstance.OpenPanel(PanelType.CharacterDescription);
            charDescPanel.UpdateCharacterDescription(_character);
        }
    }   
}
