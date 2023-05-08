using System;
using System.Collections.Generic;
using Inventory;
using Inventory_Items;
using RoguelikeMap.Points;
using RoguelikeMap.Points.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace RoguelikeMap.Panels
{
    public class EventPanel : Panel
    {
        [SerializeField] 
        private TMP_Text _eventText;
        [SerializeField] 
        private List<Button> _answerButtons;
        [SerializeField] 
        private Button _skipButton;

        private EventInfo _eventInfo;
        private Random _random;

        public event Action<IReadOnlyList<ItemData>> OnLookForLoot;
        public event Action<IReadOnlyList<IBattleCharacterInfo>> OnStartBattle;

        public override void SetInfo(PointModel model)
        {
            if (model is not EventPointModel eventPointModel)
                throw new ArgumentException("Is not valid PointInfo");
            SetEventInfo(eventPointModel.StartEventInfo);
        }

        private void SetEventInfo(EventInfo info)
        {
            _eventInfo = info;
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
            if(_eventInfo.IsHaveItems)
                OnLookForLoot?.Invoke(_eventInfo.ItemsId);
        }
    }
}