using System.Collections.Generic;
using System;
using System.Linq;
using OrderElimination.Start;
using RoguelikeMap;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace OrderElimination
{
    public class Squad : SerializedMonoBehaviour, ISquad
    {
        [OdinSerialize]
        [ShowInInspector]
        private List<Character> _testSquadMembers;
        [SerializeField]
        private SquadButtonTouchRace _proccesClick;
        [SerializeField]
        private Button _squadButton;
        private SquadInfo _squadInfo;
        private SquadModel _model;
        private SquadView _view;
        private SquadPresenter _presenter;
        private SquadCommander _commander;
        private CharactersMediator _charactersMediator;
        public event Action<Squad> OnSelected;
        public Point Point => _presenter.Point;
        public int AmountOfCharacters => _model.AmountOfMembers;
        public List<Character> Members => _testSquadMembers;
        
        [Inject]
        private void Construct(CharactersMediator charactersMediator, SquadCommander commander)
        {
            _charactersMediator = charactersMediator;
            _commander = commander;
            _commander.OnSelected += SetSquadMembers;
            _commander.OnHealAccept += HealCharacters;
        }

        private void Awake()
        {
            _model = new SquadModel(_testSquadMembers);
            _view = new SquadView(transform);
            _presenter = new SquadPresenter(_model, _view, null);
            _proccesClick.Clicked += Select;
            _proccesClick.Holded += _commander.ShowSquadMembers;
            _squadButton.onClick.AddListener(() => _commander.ShowSquadMembers());
        }

        private void Start()
        {
            _testSquadMembers = SquadMediator.CharacterList.ToList();
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

        public void StartAttack() => _commander.StartAttack();

        public void ShowSquadMembers() => _commander.ShowSquadMembers();

        public void VisitSafeZonePoint(Point point, DialogWindowData data)
        {
            SetPoint(point);
            _commander.ShowSafeZoneImage(data);
        }

        public void VisitShopPoint(Point point, DialogWindowData data)
        {
            SetPoint(point);
            _commander.ShowShopImage(data);
        }

        public void VisitBattlePoint(Point point, DialogWindowData data)
        {
            SetPoint(point);
            _commander.ShowBattleImage(data);
        }

        public void VisitEventPoint(Point point, DialogWindowData data)
        {
            SetPoint(point);
            _commander.ShowEventImage(data);
        }

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
            _proccesClick.Holded -= _commander.ShowSquadMembers;
        }
    }
}