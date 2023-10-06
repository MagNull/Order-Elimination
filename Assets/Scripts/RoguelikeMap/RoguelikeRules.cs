using System;
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
            _map.LoadPoints();
            var pointIndex = _saver.GetPointIndex();
            var point = _map.GetPointByIndex(pointIndex);
            var isEnd = CheckLoadAfterBattlePoint(point);
            if(!isEnd)
                await _map.MoveToPoint(point);
        }

        private bool CheckLoadAfterBattlePoint(Point point)
        {
            if (!_mediator.Contains<BattleResults>("battle results")
                || _mediator.Get<BattleResults>("battle results").BattleOutcome is not BattleOutcome.Win) return false;
            if (point.Model is FinalBattlePointModel)
            {
                GameEnd(true);
                return true;
            }

            _squad.MoveWithoutAnimation(point.Model.position);
            _mediator.Unregister("battle results");
            _saver.PassPoint();
            return false;
        }

        private void GameEnd(bool isVictory)
        {
            if (isVictory)
                _victoryPanel.Open();
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