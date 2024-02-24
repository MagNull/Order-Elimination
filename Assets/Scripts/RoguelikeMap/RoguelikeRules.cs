using System;
using System.Linq;
using OrderElimination;
using OrderElimination.Battle;
using OrderElimination.SavesManagement;
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
                LevelComplete(false);
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
                LevelComplete(true);
                _mediator.Unregister(MediatorRegistration.BattleResults);
                return true;
            }

            _mediator.Unregister(MediatorRegistration.BattleResults);
            _saver.PassPoint();
            return false;
        }

        private void LevelComplete(bool isVictory)
        {
            if (!isVictory)
            {
                _losePanel.Open();
                Destroy(_mediator.gameObject);
                return;
            }

            if (CheckNextMap())
            {
                ReloadScene();
                return;
            }

            _victoryPanel.Open();
            Destroy(_mediator.gameObject);
        }

        private bool CheckNextMap()
        {
            if (_map.CurrentMap.NextMaps.Count == 0)
                return false;

            var nextMapIndex = UnityEngine.Random.Range(0, _map.CurrentMap.NextMaps.Count);
            var nextMap = _map.CurrentMap.NextMaps[nextMapIndex];

            var currentRunProgress = _mediator.Get<IPlayerProgressManager>(MediatorRegistration.ProgressManager)
                    .GetPlayerProgress().CurrentRunProgress;
            currentRunProgress.CurrentMap = nextMap.AssetId;
            currentRunProgress.CurrentPointId = Guid.Empty;
            currentRunProgress.PassedPoints.Clear();
            return true;
        }

        public void LoadStartScene()
        {
            var sceneTransition = _objectResolver.Resolve<SceneTransition>();
            sceneTransition.LoadStartSessionMenu();
        }

        private void ReloadScene()
        {
            var sceneTransition = _objectResolver.Resolve<SceneTransition>();
            sceneTransition.LoadRoguelikeMap();
        }
    }
}