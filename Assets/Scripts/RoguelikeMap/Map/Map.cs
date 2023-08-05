using System.Collections.Generic;
using System.Linq;
using OrderElimination;
using OrderElimination.Battle;
using RoguelikeMap.Points;
using RoguelikeMap.Points.Models;
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
        
        private List<Point> _points;
        private IMapGenerator _mapGenerator;
        private Squad _squad;
        private bool _isSquadSelected;
        private ScenesMediator _mediator;
        private Wallet _wallet;
        private IObjectResolver _objectResolver;

        public static string SquadPositionKey = "SquadPosition";

        [Inject]
        private void Construct(IMapGenerator mapGenerator, ScenesMediator mediator,
            Wallet wallet, Squad squad, IObjectResolver objectResolver)
        {
            _mapGenerator = mapGenerator;
            _squad = squad;
            _mediator = mediator;
            _wallet = wallet;
            _objectResolver = objectResolver;
        }

        private void Start()
        {
            _points = _mapGenerator.GenerateMap();
            SetSquadPosition();
        }

        private void SetSquadPosition()
        {
            var point = _points[0];
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
            PlayerPrefs.SetInt("MoneyAfterGameEnd", _wallet.Money + 1000);
            Destroy(_mediator.gameObject);
        }
    }
}