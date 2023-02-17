using System.Collections.Generic;
using System;
using OrderElimination.Start;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UIManagement;
using UnityEngine;
using UnityEngine.EventSystems;
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
        public static event Action<Squad> Selected;
        public static event Action<Squad> Unselected;
        public static event Action onMove;
        public event Action<Squad> onActiveSquadPanel;
        public IPoint Point => _presenter.Point;
        public int AmountOfCharacters => _model.AmountOfMembers;
        public List<Character> Members => _testSquadMembers;
        public bool AlreadyMove { get; private set; }
        
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
            _view.onEndAnimation += StartAttack;
            SavesMenu.ExitSavesWindow += SetActiveButtonOnOrderPanel;
            Settings.ExitSettingsWindow += SetActiveButtonOnOrderPanel;
        }

        public void Add(Character member) => _model.Add(member);

        public void Remove(Character member) => _model.RemoveCharacter(member);

        public void Move(IPoint point)
        {
            AlreadyMove = true;
            SetPlanetPoint(point);
            _model.Move(point);
        }

        private void ActiveSquadPanel(HoldableButton button, float holdTime)
        {
            onActiveSquadPanel?.Invoke(this);
        }

        public void StartAttack()
        {
            onMove?.Invoke();
            // if (!Point.HasEnemy)
            //     return;
            //_commander.Set(this, Point);
        }
        
        public void VisitSafeZonePoint(){}
        
        public void VisitShopPoint(){}
        
        public void VisitBattlePoint(){}
        
        public void VisitEventPoint(){}

        public void SetAlreadyMove(bool isAlreadyMove)
        {
            AlreadyMove = isAlreadyMove;
            if(!isAlreadyMove)
                _view.OnReadyMove();
        }

        public void SetOrderButton(HoldableButton button)
        {
            _buttonOnOrderPanel = button;
            _buttonOnOrderPanel.Clicked -= OnClicked;
            _buttonOnOrderPanel.Clicked += OnClicked;
            _buttonOnOrderPanel.Holded -= ActiveSquadPanel;
            _buttonOnOrderPanel.Holded += ActiveSquadPanel;
        }

        private void SetPlanetPoint(IPoint point)
        {
            _presenter.UpdatePlanetPoint(point);
        }

        public void SetActiveButtonOnOrderPanel(bool isActive)
        {
            _buttonOnOrderPanel.gameObject.SetActive(isActive);
        }

        public void OnClicked(HoldableButton button) => Select();

        public void Select()
        {
            if (AlreadyMove)
                return;
            Selected?.Invoke(this);
            _model.Select();
        }

        public void Unselect()
        {
            Unselected?.Invoke(null);
            _model.Unselect();
        }

        public void DistributeExperience(float expirience) => _model.DistributeExpirience(expirience);

        private void OnEnable()
        {
            _presenter.Subscribe();
        }

        private void OnDisable()
        {
            _presenter.Unsubscribe();
            _view.onEndAnimation -= StartAttack;
            SavesMenu.ExitSavesWindow -= SetActiveButtonOnOrderPanel;
            Settings.ExitSettingsWindow -= SetActiveButtonOnOrderPanel;
        }

        private void OnMouseDown()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("Blocked");
                return;
            }
            Select();
        }
    }
}