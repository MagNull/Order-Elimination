﻿using System.Linq;
using OrderElimination;
using OrderElimination.Battle;
using RoguelikeMap.Map;
using RoguelikeMap.Points;
using RoguelikeMap.Points.Models;
using RoguelikeMap.SquadInfo;
using RoguelikeMap.UI;
using UnityEngine;
using VContainer;

namespace RoguelikeMap
{
    public class RoguelikeRules : MonoBehaviour
    {
        [SerializeField] private Panel _losePanel;
        [SerializeField] private Panel _victoryPanel;

        private Map.Map _map;
        private Squad _squad;
        private ScenesMediator _mediator;
        private SquadPositionSaver _saver;
        private IObjectResolver _objectResolver;

        [Inject]
        public void Construct(Map.Map map, Squad squad, ScenesMediator scenesMediator,
            SquadPositionSaver saver, IObjectResolver objectResolver)
        {
            _map = map;
            _squad = squad;
            _mediator = scenesMediator;
            _saver = saver;
            _objectResolver = objectResolver;
        }

        public async void Start()
        {
            _squad.Initialize();
            if (CheckSquadMembers())
            {
                GameEnd(false);
                return;
            }
            _map.LoadPoints();
            var pointIndex = _saver.GetPointIndex();
            var point = _map.GetPointById(pointIndex);
            if (CheckLoadAfterBattlePoint(point))
                return;
            await _map.MoveToPoint(point);
        }

        private bool CheckSquadMembers()
        {
            return _squad.Members.All(x => x.CurrentHealth <= 0);
        }

        private bool CheckLoadAfterBattlePoint(Point point)
        {
            if (!_mediator.Contains<BattleResults>(MediatorRegistration.BattleResults)
                || _mediator.Get<BattleResults>(MediatorRegistration.BattleResults).BattleOutcome is not BattleOutcome.Win)
                return false;
            if (point.Model is FinalBattlePointModel)
            {
                GameEnd(true);
                return true;
            }

            _mediator.Unregister(MediatorRegistration.BattleResults);
            _saver.PassPoint();
            return false;
        }

        private void GameEnd(bool isVictory)
        {
            if (isVictory)
            {
                _victoryPanel.Open();
            }
            else
                _losePanel.Open();

            Destroy(_mediator.gameObject);
        }

        public void LoadStartScene()
        {
            var sceneTransition = _objectResolver.Resolve<SceneTransition>();
            sceneTransition.LoadStartSessionMenu();
        }
    }
}