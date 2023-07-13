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

        private EventPointGraph _eventGraph;
        private Inventory _inventory;

        public event Action<IReadOnlyList<IGameCharacterTemplate>> OnStartBattle;
        public event Action<bool> OnSafeEventVisit;
        public event Action<bool> OnBattleEventVisit;

        [Inject]
        public void Construct(Inventory inventory)
        {
            _inventory = inventory;
        }
        
        public void Initialize(EventPointGraph graph)
        {
            graph.ResetGraph();
            _eventGraph = graph;
            _eventGraph.OnEventEnd += FinishEvent;
            LoadEventInfo();
        }

        private void SetActiveAnswers(bool isActive)
        {
            _skipButton.gameObject.SetActive(!isActive);
            
            foreach(var button in _answerButtons)
                button.gameObject.SetActive(isActive);
        }

        private void UpdateAnswersText()
        {
            SetActiveAnswers(true);
            for (var i = 0; i < _eventGraph.Current.Answers.Count; i++)
            {
                var buttonText = _answerButtons[i].GetComponentInChildren<TMP_Text>();
                buttonText.text = _eventGraph.Current.Answers[i];
            }
        }

        private void LoadEventInfo()
        {
            _sprite.sprite = _eventGraph.Current.Sprite;
            _eventText.text = _eventGraph.Current.Text;
            if (!_eventGraph.Current.IsFork)
            {
                SetActiveAnswers(false);
                return;
            }
            UpdateAnswersText();
        }

        private void FinishEvent()
        {
            if (_eventGraph.Current.IsEnd)
                EventEnd();
            else if (_eventGraph.Current.IsBattle)
                EventEndWithBattle();
            Close();
        }

        private void UpdateEventInfo(int buttonIndex)
        {
            _eventGraph.NextNode(buttonIndex);
        }

        public void ClickAnswer(int buttonIndex)
        {
            UpdateEventInfo(buttonIndex);
            LoadEventInfo();
        }

        private void EventEnd()
        {
            if (!_eventGraph.Current.IsHaveItems) 
                return;
            foreach (var itemData in _eventGraph.Current.ItemsId)
            {
                var item = ItemFactory.Create(itemData);
                _inventory.AddItem(item);
            }
        }

        private void EventEndWithBattle()
        {
            OnStartBattle?.Invoke(_eventGraph.Current.Enemies);
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
            if (_eventGraph is null)
            {
                Logging.LogException(new MissingFieldException());
                return;
            }
            if (_eventGraph.IsContainsBattle)
                OnBattleEventVisit?.Invoke(isPlay);
            else
                OnSafeEventVisit?.Invoke(isPlay);
        }

        private void OnDisable()
        {
            _eventGraph.OnEventEnd -= FinishEvent;
        }
    }
}