using System.Collections.Generic;
using UnityEngine.UI;
using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using VContainer;

namespace OrderElimination
{
    public class Squad : SerializedMonoBehaviour, ISelectable, IMovable
    {
        [OdinSerialize]
        [ShowInInspector]
        private List<Character> _testSquadMembers; 
        
        private SquadInfo _squadInfo; 
        private SquadModel _model;
        private SquadView _view;
        private SquadPresenter _presenter;
        private SquadCommander _commander;
        private Button _rectangleOnPanelButton;
        private CharactersMediator _charactersMediator;
        public static event Action<Squad> Selected;
        public static event Action<Squad> Unselected;
        public PlanetPoint PlanetPoint => _presenter.PlanetPoint;
        public int AmountOfCharacters => _model.AmountOfMembers;
        public IReadOnlyList<Character> Members => _model.Members;
        public bool AlreadyMove = false;


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
        }

        public void Add(Character member) => _model.Add(member);

        public void Remove(Character member) => _model.RemoveCharacter(member);

        public void Move(PlanetPoint planetPoint)
        {
            AlreadyMove = true;
            PlanetPoint?.RemoveSquad();
            planetPoint?.AddSquad();
            SetPlanetPoint(planetPoint);
            SetOrderButtonCharacteristics(true);
            _model.Move(planetPoint);
        }

        public void StartAttack()
        {
            _commander.Set(this, PlanetPoint);
            var order =_commander.CreateAttackOrder();
            order.Start();
        }

        public void SetOrderButton(Button image)
        {
            _rectangleOnPanelButton = image;
            _view.SetButtonOnOrder(image);
        }

        public void SetOrderButtonCharacteristics(bool isActive)
            => _view.SetButtonCharacteristics(isActive);

        private void SetPlanetPoint(PlanetPoint planetPoint)
        {
            _presenter.UpdatePlanetPoint(planetPoint);
        }

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
        }

        private void OnMouseDown() => Select();
    }
}