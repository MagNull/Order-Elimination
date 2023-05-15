using System;
using System.Collections.Generic;
using System.Linq;
using OrderElimination;
using RoguelikeMap.Panels;
using RoguelikeMap.Points;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using VContainer;

namespace RoguelikeMap.SquadInfo
{
    public class Squad : SerializedMonoBehaviour, ISquad
    {
        [OdinSerialize]
        [ShowInInspector]
        private List<Character> _testSquadMembers;
        [SerializeField]
        private SquadButtonTouchRace _proccesClick;
        private OrderElimination.SquadInfo _squadInfo;
        private SquadModel _model;
        private SquadView _view;
        private SquadPresenter _presenter;
        private SquadCommander _commander;
        private PanelGenerator _panelGenerator;
        private CharactersMediator _charactersMediator;
        public event Action<Squad> OnSelected;
        public Point Point => _presenter.Point;
        public int AmountOfCharacters => _model.AmountOfMembers;
        public List<Character> Members => _testSquadMembers;
        
        [Inject]
        private void Construct(CharactersMediator charactersMediator, SquadCommander commander,
            PanelGenerator panelGenerator)
        {
            _charactersMediator = charactersMediator;
            _commander = commander;
            _panelGenerator = panelGenerator;
            
            _commander.OnSelected += SetSquadMembers;
            _commander.OnHealAccept += HealCharacters;
        }

        private void Awake()
        {
            _model = new SquadModel(_testSquadMembers);
            _view = new SquadView(transform);
            _presenter = new SquadPresenter(_model, _view, null);
            _proccesClick.Clicked += Select;
        }

        private void Start()
        {
            if(SquadMediator.CharacterList is not null)
                SetSquadMembers(SquadMediator.CharacterList.ToList());
            SetPanel();
            foreach(var member in _testSquadMembers)
                member.Upgrade(SquadMediator.Stats);
            SquadMediator.SetStatsCoefficient(new List<int>(){0, 0, 0, 0, 0});
        }

        public void Move(Point point)
        {
            SetPoint(point);
            _model.Move(point);
        }

        private void HealCharacters() => _model.HealCharacters();

        private void SetSquadMembers(List<Character> squadMembers)
        {
            _model.SetSquadMembers(squadMembers);
            _testSquadMembers = squadMembers;
        }

        private void SetPanel()
        {
            var squadMemberPanel = _panelGenerator.GetSquadMembersPanel();
            squadMemberPanel.UpdateMembers(Members);
            _view.SetPanel(squadMemberPanel);
        }

        public void StartAttack() => _commander.StartAttack();

        private void SetActiveSquadMembers(bool isActive) => _view.SetActivePanel(isActive);

        public void Visit(Point point) => SetPoint(point);

        private void SetPoint(Point point)
        {
            _commander.Set(this, point);
            _model.Move(point);
            _presenter.UpdatePlanetPoint(point);
        }

        public void Select()
        {
            Debug.Log("Squad selected");
            OnSelected?.Invoke(this);
            _model.Select();
        }

        public void DistributeExperience(float expirience) => _model.DistributeExperience(expirience);

        private void OnEnable()
        {
            _presenter.Subscribe();
        }

        private void OnDisable()
        {
            _presenter.Unsubscribe();
            _proccesClick.Clicked -= Select;
        }
    }
}