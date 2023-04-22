using System;
using System.Collections.Generic;
using RoguelikeMap.Points.VarietiesPoints.Infos;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        public event Action<IReadOnlyList<int>> OnEventEnd;

        public override void SetPointInfo(VarietiesPointInfo pointInfoInfo)
        {
            if (pointInfoInfo is not EventPointInfo eventPointInfo)
                throw new ArgumentException("Is not valid PointInfo");
            _eventInfo = eventPointInfo.StartEventInfo;
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
            if (_eventInfo.IsEnd)
            {
                EventEnd();
                Close();
                return;
            }
            
            var text = _eventInfo.Text;
            var possibleAnswers = GetPossibleAnswers();
            UpdateEventText(text, possibleAnswers);
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

        private void EventEnd()
        {
            if(_eventInfo.IsHaveItems)
                OnEventEnd?.Invoke(_eventInfo.ItemsId);
        }
    }
}