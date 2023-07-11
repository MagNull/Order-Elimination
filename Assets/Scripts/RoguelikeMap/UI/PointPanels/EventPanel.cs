using System;
using System.Collections.Generic;
using GameInventory;
using GameInventory.Items;
using OrderElimination;
using RoguelikeMap.Points.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using Random = System.Random;

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

        private EventInfo _eventInfo;
        private Random _random;
        private Inventory _inventory;

        public bool IsContainsBattle { get; private set; }
        public event Action<IReadOnlyList<IGameCharacterTemplate>> OnStartBattle;
        public event Action<bool> OnSafeEventVisit;
        public event Action<bool> OnBattleEventVisit;

        [Inject]
        public void Construct(Inventory inventory)
        {
            _inventory = inventory;
        }
        
        public void SetEventInfo(EventInfo info, bool isContainsBattle)
        {
            _eventInfo = info;
            IsContainsBattle = isContainsBattle;
            LoadEventText();
        }

        private void UpdateEventText(string eventText, IReadOnlyList<string> answers = null)
        {
            _eventText.text = eventText;
            UpdateAnswersText(answers);
        }

        private void SetActiveAnswers(bool isActive)
        {
            _skipButton.gameObject.SetActive(!isActive);
            
            foreach(var button in _answerButtons)
                button.gameObject.SetActive(isActive);
        }

        private void UpdateAnswersText(IReadOnlyList<string> answers = null)
        {
            if (answers is null)
            {
                SetActiveAnswers(false);
                return;
            }
            
            SetActiveAnswers(true);
            for (var i = 0; i < _answerButtons.Count; i++)
            {
                var buttonText = _answerButtons[i].GetComponentInChildren<TMP_Text>();
                buttonText.text = answers[i];
            }
        }

        private void LoadEventText()
        {
            if (IsEventEnd())
                return;
            if (_eventInfo.IsRandomFork)
                LoadRandomFork();
            _sprite.sprite = _eventInfo.Sprite;
            var text = _eventInfo.Text;
            var possibleAnswers = GetPossibleAnswers();
            UpdateEventText(text, possibleAnswers);
        }

        private void LoadRandomFork()
        {
            _random ??= new Random();
            var index = _random.Next(_eventInfo.NextStages.Count);
            _eventInfo = _eventInfo.NextStages[index];
        }

        private bool IsEventEnd()
        {
            if (!_eventInfo.IsEnd && !_eventInfo.IsBattle)
                return false;
            
            if (_eventInfo.IsEnd)
                EventEnd();
            else if (_eventInfo.IsBattle)
                EventEndWithBattle();

            Close();
            return true;
        }

        private void UpdateEventInfo(int buttonIndex = -1)
        {
            _eventInfo = buttonIndex switch
            {
                -1 => _eventInfo.NextStage,
                0 => _eventInfo.NextStages[0],
                1 => _eventInfo.NextStages[1],
                _ => throw new ArgumentException("Is not valid button index")
            };
        }

        private IReadOnlyList<string> GetPossibleAnswers()
        {
            return !_eventInfo.IsFork ? null : _eventInfo.Answers;
        }

        public void OnClickAnswer(int buttonIndex)
        {
            UpdateEventInfo(buttonIndex);
            LoadEventText();
        }

        public void OnClickSkipButton()
        {
            UpdateEventInfo();
            LoadEventText();
        }

        private void EventEndWithBattle()
        {
            OnStartBattle?.Invoke(_eventInfo.Enemies);
        }

        private void EventEnd()
        {
            if (_eventInfo.IsHaveItems)
            {
                foreach (var itemData in _eventInfo.ItemsId)
                {
                    var item = ItemFactory.Create(itemData);
                    _inventory.AddItem(item);
                }
            }
        }

        public override void Open()
        {
            base.Open();
            VisitEventInvoke(true);
        }
        
        public override void Close()
        {
            VisitEventInvoke();
            base.Close();
        }
        
        private void VisitEventInvoke(bool isPlay = false)
        {
            if (IsContainsBattle)
                OnBattleEventVisit?.Invoke(isPlay);
            else
                OnSafeEventVisit?.Invoke(isPlay);
        }
    }
}