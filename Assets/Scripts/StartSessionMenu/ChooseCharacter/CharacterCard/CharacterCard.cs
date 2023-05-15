using OrderElimination;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
<<<<<<< HEAD:Assets/Scripts/StartSessionMenu/ChooseCharacter/CharacterCard.cs
using RoguelikeMap;
using UIManagement;
using UIManagement.Elements;
=======
>>>>>>> StrategyMap:Assets/Scripts/StartSessionMenu/ChooseCharacter/CharacterCard/CharacterCard.cs

namespace StartSessionMenu.ChooseCharacter.CharacterCard
{
    public class CharacterCard : MonoBehaviour
    {
        protected Character _character;
        [FormerlySerializedAs("_isSelected")] 
        public bool IsSelected;
        
        [SerializeField] 
        protected Image _cardImage;
        [SerializeField] 
<<<<<<< HEAD:Assets/Scripts/StartSessionMenu/ChooseCharacter/CharacterCard.cs
        private Button _button;
        private HoldableButton _holdableButton;

        public bool _isSelected;
=======
        protected Button _button;
        
        public Character Character => _character;
        public Button Button => _button;
>>>>>>> StrategyMap:Assets/Scripts/StartSessionMenu/ChooseCharacter/CharacterCard/CharacterCard.cs

        public virtual void InitializeCard(Character character)
        {
            _character = character;
            _cardImage.sprite = character.GetViewAvatar();
<<<<<<< HEAD:Assets/Scripts/StartSessionMenu/ChooseCharacter/CharacterCard.cs
            _healthBar = GetComponentInChildren<HealthBar>();
            _healthBar?.SetMaxHealth(character.GetBattleStats().UnmodifiedHealth);
            if(_cardCost is not null)
                _cardCost.text = cost.ToString() + "$";
            _holdableButton = GetComponent<HoldableButton>();
            if (_holdableButton)
                _holdableButton.Holded += OnHold;
=======
>>>>>>> StrategyMap:Assets/Scripts/StartSessionMenu/ChooseCharacter/CharacterCard/CharacterCard.cs
            _button.onClick = new Button.ButtonClickedEvent();
        }

        public virtual void Select()
        {
            IsSelected = !IsSelected;
        }
<<<<<<< HEAD:Assets/Scripts/StartSessionMenu/ChooseCharacter/CharacterCard.cs

        private void OnHold(HoldableButton b, float t)
        {
            var charDescPanel =
                (CharacterDescriptionPanel) UIController.SceneInstance.OpenPanel(PanelType.CharacterDescription);
            charDescPanel.UpdateCharacterDescription(_character);
        }
    }   
}
=======
    }
}
>>>>>>> StrategyMap:Assets/Scripts/StartSessionMenu/ChooseCharacter/CharacterCard/CharacterCard.cs
