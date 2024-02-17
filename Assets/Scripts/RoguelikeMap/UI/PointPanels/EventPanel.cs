using System;
using System.Collections.Generic;
using System.Linq;
using Events;
using GameInventory;
using GameInventory.Items;
using OrderElimination;
using OrderElimination.MacroGame;
using OrderElimination.UI;
using RoguelikeMap.SquadInfo;
using StartSessionMenu;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace RoguelikeMap.UI.PointPanels
{
    public class EventPanel : Panel
    {
        [SerializeField]
        private TMP_Text _eventText;
        [SerializeField]
        private List<Button> _answerButtons;
        [SerializeField]
        private Button _skipButton;
        [SerializeField]
        private Image _sprite;
        [SerializeField]
        private Transform _skipButtonParent;
        [SerializeField]
        private Transform _buttonsParent;

        private Inventory _inventory;
        private Squad _squad;
        private Wallet _wallet;
        private bool _isContainsBattle = false;
        private bool _isSwap = false;

        public event Action<int> OnAnswerClick;
        public event Action<BattleNode> OnStartBattle;
        public event Action<bool> OnSafeEventVisit;
        public event Action<bool> OnBattleEventVisit;

        [Inject]
        public void Construct(Inventory inventory, Squad squad, Wallet wallet)
        {
            _inventory = inventory;
            _squad = squad;
            _wallet = wallet;
        }

        private void SetActiveAnswers(bool isActive)
        {
            foreach (var button in _answerButtons)
            {
                button.gameObject.SetActive(isActive);
                button.DOInterectable(true);
            }
        }

        public bool CheckItem(ItemData itemData)
        {
            return _inventory.Contains(itemData);
        }

        public bool CheckCharacter(CharacterTemplate characterTemplate)
        {
            var characters = _squad.Members.Any(x => x.CharacterData == characterTemplate);
            return characters;
        }

        public void SpendItems(IEnumerable<ItemData> itemsDatas)
        {
            foreach (var itemData in itemsDatas)
            {
                SpendItem(itemData);
            }
        }

        public void SpendItem(ItemData itemData)
        {
            if (CheckItem(itemData))
            {
                _inventory.RemoveItem(itemData);
                Debug.Log("CheckItem");
            }
        }

        public void HealCharacters(int percentage)
        {
            _squad.HealCharactersByPercentage(percentage);
        }

        public void DamageCharacters(int percentage)
        {
            _squad.DamageCharactersByPercentage(percentage);
        }

        public void AddItems(IEnumerable<ItemData> itemsDatas)
        {
            foreach (var itemData in itemsDatas)
            {
                AddItem(itemData);
            }
        }

        public void AddItem(ItemData itemData)
        {
            var item = ItemFactory.Create(itemData);
            _inventory.AddItem(item);
        }

        public void AddMoney(int money)
        {
            _wallet.Money += money;
        }

        public void SetInteractableAnswer(int answerIndex, bool isInteractable)
        {
            _answerButtons[answerIndex].gameObject.SetActive(isInteractable);
        }

        public void UpdateAnswersText(IReadOnlyList<string> answers)
        {
            SetActiveSkipButton(false);
            SetActiveAnswers(false);

            while (answers.Count > _answerButtons.Count)
                _answerButtons.Add(Instantiate(_answerButtons[0], _buttonsParent));

            for (var i = 0; i < answers.Count; i++)
            {
                _answerButtons[i].gameObject.SetActive(true);
                var buttonText = _answerButtons[i].GetComponentInChildren<TMP_Text>();
                buttonText.text = answers[i];
            }
        }

        public void CheckActiveAnswerButtons()
        {
            var activeAnswerButtons = _answerButtons.Where(x => x.gameObject.activeSelf);
            if (activeAnswerButtons.Count() != 1) return;
            _answerButtons[0].transform.SetParent(_skipButtonParent);
            var rectTransformButton = _answerButtons[0].transform as RectTransform;
            rectTransformButton.sizeDelta = new Vector2(1715, 80);
            _isSwap = true;
        }

        public void UpdateSprite(Sprite sprite)
        {
            if (sprite is null)
                return;
            _sprite.sprite = sprite;
        }

        public void UpdateText(string text)
        {
            _eventText.text = text;
            SetActiveSkipButton(true);
            SetActiveAnswers(false);
        }

        private void SetActiveSkipButton(bool isActive)
        {
            _skipButton.gameObject.SetActive(isActive);
        }

        public void FinishEvent(IEnumerable<ItemData> items = null,
            IEnumerable<CharacterTemplate> characters = null)
        {
            if (items is not null)
                AddItemsToInventory(items);
            if (characters is not null)
                _squad.AddMembers(GameCharactersFactory.CreateGameCharacters(characters));
            Close();
        }

        private void AddItemsToInventory(IEnumerable<ItemData> items)
        {
            foreach (var itemData in items)
            {
                var item = ItemFactory.Create(itemData);
                _inventory.AddItem(item);
            }
        }

        public void FinishEventWithBattle(BattleNode battleNode)
        {
            OnStartBattle?.Invoke(battleNode);
        }

        public void ClickAnswer(int buttonIndex)
        {
            if (_isSwap)
            {
                _answerButtons[0].transform.SetParent(_buttonsParent);
                _answerButtons[0].transform.SetSiblingIndex(0);
                _isSwap = false;
            }
            OnAnswerClick?.Invoke(buttonIndex);
        }

        private void PlayEventMusic(bool isPlay)
        {
            if (_isContainsBattle)
                OnBattleEventVisit?.Invoke(isPlay);
            else
                OnSafeEventVisit?.Invoke(isPlay);
        }

        public void Open(bool isContainBattle)
        {
            _isContainsBattle = isContainBattle;
            Open();
        }

        public override void Open()
        {
            base.Open();
            PlayEventMusic(true);
        }

        public override void Close()
        {
            PlayEventMusic(false);
            base.Close();
        }

        public void ResetEvent()
        {
            OnAnswerClick = null;
        }
    }
}