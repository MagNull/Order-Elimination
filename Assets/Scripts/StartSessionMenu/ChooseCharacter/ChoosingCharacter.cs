using System;
using System.Collections.Generic;
using DG.Tweening;
using UIManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OrderElimination
{
    public class ChoosingCharacter : UIPanel
    {
        [SerializeField]
        private Transform _selected;
        [SerializeField]
        private Transform _notSelected;
        [SerializeField]
        private Button _startAttackButton;

        [SerializeField]
        private GameObject _characterButtonPref;
        [SerializeField]
        private MoneyCounter _uiCounter;
        [SerializeField]
        private MetaShop _metaShop;

        private Wallet _wallet;

        [SerializeField]
        private List<Character> _characters;

        public override PanelType PanelType => PanelType.SquadMembers;
        public event Action<List<Character>> OnSelected;

        private List<Character> _selectedCharacters;
        private List<Character> _unselectedCharacters;

        private void Start()
        {
            CreatePanel();
            _unselectedCharacters = _characters;
        }

        public void SetWallet(Wallet wallet)
        {
            _wallet = wallet;
        }

        public void CreatePanel()
        {
            _uiCounter?.Initialize(_wallet);

            _selectedCharacters = new List<Character>();

            Debug.Log("Da");
            foreach (var info in _characters)
            {
                var characterCard = Instantiate(_characterButtonPref, _notSelected);
                var characterCardInfo = characterCard.GetComponent<CharacterCard>();
                characterCardInfo.InitializeCard(info, 400);
                UnityAction act = () => SelectCharacter(characterCardInfo);
                characterCardInfo.Button.onClick.AddListener(act);
            }
        }

        public void SelectCharacter(CharacterCard card)
        {
            // TODO(coder): Подумать как разобраться с заглушкой wallet
            if (_wallet is null)
                _wallet = new Wallet(10000);
            if (!card._isSelected && _wallet.Money - card.Cost >= 0)
            {
                _wallet.SubtractMoney(card.Cost);
                card.transform.SetParent(_selected);
                _selectedCharacters.Add(card.Character);
                _unselectedCharacters.Remove(card.Character);
                card.transform.SetSiblingIndex(_selectedCharacters.Count - 1);
                card.Select();
            }
            else if (card._isSelected)
            {
                _wallet.AddMoney(card.Cost);
                card.transform.SetParent(_notSelected);
                _unselectedCharacters.Add(card.Character);
                _selectedCharacters.Remove(card.Character);
                card.Select();
            }
        }

        public void UpdateCharacterInfo(List<Character> characters, bool isInteractableAttackButton = false)
        {
            _characters = characters;
            _unselectedCharacters = characters;
            _startAttackButton.interactable = isInteractableAttackButton;
        }

        public override void Close()
        {
            OnSelected?.Invoke(_unselectedCharacters);
            transform.DOMoveX(UIController.StartPosition, 0.1f).OnStepComplete(() => base.Close());
        }

        public void SaveCharacters()
        {
            if (_selectedCharacters.Count <= 0)
                return;

            SquadMediator.SetCharacters(_selectedCharacters);
        }
    }
}