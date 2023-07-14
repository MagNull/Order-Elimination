using System;
using System.Collections.Generic;
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
        public event Action<IEnumerable<IGameCharacterTemplate>> OnStartBattle;
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
            _skipButton.gameObject.SetActive(!isActive);

            foreach (var button in _answerButtons)
            {
                button.gameObject.SetActive(isActive);
                button.DOInterectable(true);
            }
        }

        public void ResetEvent()
        {
            OnAnswerClick = null;
        }

        public bool CheckItem(ItemData itemData)
        {
            Logging.Log("Check inventory contains item");
            return false;
        }

        public void SetInteractableAnswer(int answerIndex, bool isInteractable)
        {
            _answerButtons[answerIndex].DOInterectable(isInteractable);
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

        public void FinishEvent(IEnumerable<ItemData> items = null, 
            IEnumerable<CharacterTemplate> characters = null)
        {
            if (items is not null)
                Logging.Log("add items");
                //AddItemsToInventory(items);
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