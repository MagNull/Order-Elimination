using System;
using System.Collections.Generic;
using GameInventory;
using GameInventory.Items;
using OrderElimination;
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
        private bool _isContainsBattle = false;
        
        public event Action<int> OnAnswerClick;
        public event Action<IEnumerable<IGameCharacterTemplate>> OnStartBattle;
        public event Action<bool> OnSafeEventVisit;
        public event Action<bool> OnBattleEventVisit;

        [Inject]
        public void Construct(Inventory inventory)
        {
            _inventory = inventory;
        }

        private void SetActiveAnswers(bool isActive)
        {
            _skipButton.gameObject.SetActive(!isActive);
            
            foreach(var button in _answerButtons)
                button.gameObject.SetActive(isActive);
        }

        public void UpdateAnswersText(IReadOnlyList<string> answers)
        {
            SetActiveAnswers(true);
            for (var i = 0; i < answers.Count; i++)
            {
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
            SetActiveAnswers(false);
        }

        public void FinishEvent(IEnumerable<ItemData> items = null)
        {
            if (items is not null)
                AddItemsToInventory(items);
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

        public void FinishEventWithBattle(IEnumerable<IGameCharacterTemplate> enemies)
        {
            OnStartBattle?.Invoke(enemies);
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
    }
}