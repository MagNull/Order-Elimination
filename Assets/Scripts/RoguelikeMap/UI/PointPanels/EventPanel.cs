using System;
using System.Collections.Generic;
using Events;
using GameInventory;
using GameInventory.Items;
using OrderElimination;
using OrderElimination.MacroGame;
using OrderElimination.UI;
using RoguelikeMap.SquadInfo;
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

        private Inventory _inventory;
        private Squad _squad;
        private bool _isContainsBattle = false;
        
        public event Action<int> OnAnswerClick;
        public event Action<BattleNode> OnStartBattle;
        public event Action<bool> OnSafeEventVisit;
        public event Action<bool> OnBattleEventVisit;

        [Inject]
        public void Construct(Inventory inventory, Squad squad)
        {
            _inventory = inventory;
            _squad = squad;
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

        public void SetInteractableAnswer(int answerIndex, bool isInteractable)
        {
            _answerButtons[answerIndex].DOInterectable(isInteractable);
        }

        public void UpdateAnswersText(IReadOnlyList<string> answers)
        {
            SetActiveSkipButton(false);
            SetActiveAnswers(false);
            for (var i = 0; i < answers.Count; i++)
            {
                _answerButtons[i].gameObject.SetActive(true);
                var buttonText = _answerButtons[i].GetComponentInChildren<TMP_Text>();
                buttonText.text = answers[i];
            }
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
            if(characters is not null)
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