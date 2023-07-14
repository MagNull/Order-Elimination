using System.Collections.Generic;
using System.Linq;
using OrderElimination;
using OrderElimination.Battle;
using RoguelikeMap.Points;
using RoguelikeMap.SquadInfo;
using RoguelikeMap.UI;
using StartSessionMenu;
using UnityEngine;
using VContainer;

namespace RoguelikeMap.Map
{
    public class Map : MonoBehaviour
    {
        [SerializeField]
        private Panel _victoryPanel;
        
        public static string SquadPositionKey = "SquadPosition";
        
        private List<Point> _points;
        private IMapGenerator _mapGenerator;
        private Squad _squad;
        private bool _isSquadSelected;
        private SquadCommander _squadCommander;
        private ScenesMediator _mediator;
        private Wallet _wallet;
        
        private IObjectResolver _objectResolver;
        public static int SaveIndex { get; private set; }

        [Inject]
        private void Construct(IMapGenerator mapGenerator, ScenesMediator mediator,
            SquadCommander squadCommander, Wallet wallet,
            Squad squad, IObjectResolver objectResolver)
        {
            _mapGenerator = mapGenerator;
            _squad = squad;
            _mediator = mediator;
            _wallet = wallet;
            _squadCommander = squadCommander;
            _objectResolver = objectResolver;
        }

        private void Start()
        {
            _points = _mapGenerator.GenerateMap();
            SetSquadPosition();
            foreach (var point in _points)
                point.OnSelected += SelectPoint;

            _squad.OnSelected += SelectSquad;
        }
        
        private void SelectSquad(Squad squad)
        {
            _isSquadSelected = true;
        }
        
        private void UnselectSquad()
        {
            _isSquadSelected = false;
        }

        private void SelectPoint(Point point)
        {
            Debug.Log("Point click" % Colorize.Red);
            if (_isSquadSelected is false)
                return;
            if(_squad.Point.NextPoints.Contains(point.Index))
                point.Visit(_squad);
            UnselectSquad();
        }

        private void SetSquadPosition()
        {
            var pointIndex = PlayerPrefs.HasKey(SquadPositionKey)
                ? PlayerPrefs.GetInt(SquadPositionKey)
                : _points.First().Index;
            var point = _points[pointIndex];
            if(_squadCommander.BattleOutcome is null or BattleOutcome.Lose)
                point.Visit(_squad);
            else if (point.Model.IsLastPoint)
                GameEnd();
            else
                _squad.Visit(point.Model);
        }

        public void ReloadMap()
        {
            PlayerPrefs.DeleteKey(SquadPositionKey);
            SetSquadPosition();
        }

        public void LoadStartScene()
        {
            var sceneTransition = _objectResolver.Resolve<SceneTransition>();
            sceneTransition.LoadStartSessionMenu();
        }

        private void GameEnd()
        {
            _victoryPanel.Open();
            PlayerPrefs.SetInt("Money", _wallet.Money + 1000);
            Destroy(_mediator.gameObject);
        }
    }
}