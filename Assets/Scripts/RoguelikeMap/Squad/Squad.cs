using System.Collections.Generic;
using System;
using System.Linq;
using OrderElimination.Start;
using RoguelikeMap;
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
        public event Action<Squad> OnSelected;
        public event Action<Squad> onActiveSquadPanel;
        public Point Point => _presenter.Point;
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
            _view.onEndAnimation += SetSquadCommander;
            SavesMenu.ExitSavesWindow += SetActiveButtonOnOrderPanel;
            Settings.ExitSettingsWindow += SetActiveButtonOnOrderPanel;
        }

        private void Start()
        {
            _testSquadMembers = SquadMediator.CharacterList.ToList();
        }

        public void Add(Character member) => _model.Add(member);

        public void Remove(Character member) => _model.RemoveCharacter(member);

        public void Move(Point point)
        {
            //AlreadyMove = true;
            SetPoint(point);
            _model.Move(point);
        }

        private bool CheckOutScreenBoundaries(Vector3 position)
        {
            var mainCamera = Camera.main;
            var screenBounds = mainCamera.ScreenToWorldPoint(
                new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
            return position.x < -screenBounds.x 
                   || position.x > screenBounds.x 
                   || position.y < -screenBounds.y 
                   || position.y > screenBounds.y;
        }

        private void ActiveSquadPanel(HoldableButton button, float holdTime)
        {
            onActiveSquadPanel?.Invoke(this);
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
            if (AlreadyMove)
                return;
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