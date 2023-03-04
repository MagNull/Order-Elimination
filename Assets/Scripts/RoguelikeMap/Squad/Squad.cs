using System.Collections.Generic;
using System;
using System.Linq;
using OrderElimination.Start;
using RoguelikeMap;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using VContainer;
using UIManagement.Elements;

namespace OrderElimination
{
    public class Squad : SerializedMonoBehaviour, ISquad
    {
        [OdinSerialize]
        [ShowInInspector]
        private List<Character> _testSquadMembers;
        private SquadInfo _squadInfo; 
        private SquadModel _model;
        private SquadView _view;
        private SquadPresenter _presenter;
        private SquadCommander _commander;
        private HoldableButton _buttonOnOrderPanel;
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
        }

        private void Awake()
        {
            _model = new SquadModel(_testSquadMembers);
            _view = new SquadView(transform);
            _presenter = new SquadPresenter(_model, _view, null);
            _view.onEndAnimation += SetSquadCommander;
            SavesMenu.ExitSavesWindow += SetActiveButtonOnOrderPanel;
            Settings.ExitSettingsWindow += SetActiveButtonOnOrderPanel;
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

        private void SetSquadCommander()
        {
            _commander.Set(this, Point);
        }

        public void StartAttack() => _commander.StartAttack();

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
            //AlreadyMove = true;
            _commander.Set(this, point);
            _model.Move(point);
            _presenter.UpdatePlanetPoint(point);
        }

        public void SetActiveButtonOnOrderPanel(bool isActive)
        {
            _buttonOnOrderPanel.gameObject.SetActive(isActive);
        }

        public void OnClicked(HoldableButton button) => Select();

        public void Select()
        {
            Debug.Log("Squad selected");
            OnSelected?.Invoke(this);
            _model.Select();
        }

        public void DistributeExperience(float expirience) => _model.DistributeExpirience(expirience);

        private void OnEnable()
        {
            _presenter.Subscribe();
        }

        private void OnDisable()
        {
            _presenter.Unsubscribe();
            _view.onEndAnimation -= SetSquadCommander;
            SavesMenu.ExitSavesWindow -= SetActiveButtonOnOrderPanel;
            Settings.ExitSettingsWindow -= SetActiveButtonOnOrderPanel;
        }

        private void OnMouseDown() => Select();
    }
}