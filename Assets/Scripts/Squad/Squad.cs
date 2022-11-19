using System.Collections.Generic;
using UnityEngine.UI;
using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace OrderElimination
{
    public class Squad : SerializedMonoBehaviour, ISelectable, IMovable
    {
        [OdinSerialize]
        [ShowInInspector]
        private List<ISquadMember> _testSquadMembers; 
        
        private SquadInfo _squadInfo; 
        private SquadModel _model;
        private SquadView _view;
        private SquadPresenter _presenter;
        [ShowInInspector]
        private PlanetPoint _planetPoint;
        private Order _order;
        private Button _rectangleOnPanelButton;
        public static event Action<Squad> Selected;
        public static event Action<Squad> Unselected;
        public PlanetPoint PlanetPoint => _planetPoint;
        public int AmountOfCharacters => _model.AmountOfMembers;
        public IReadOnlyList<ISquadMember> Members => _model.Members;

        private void Awake()
        {
            _model = new SquadModel(_testSquadMembers);
            _view = new SquadView(transform);
            _presenter = new SquadPresenter(_model, _view, null);
            _planetPoint = null;
            _order = null;
        }

        public void Add(ISquadMember member) => _model.Add(member);

        public void Remove(ISquadMember member) => _model.RemoveCharacter(member);

        public void Move(PlanetPoint planetPoint)
        {
            SetPlanetPoint(planetPoint);
            _model.Move(planetPoint);
        }

        public void SetOrder(Order order)
        {
            _order = order;
        }

        public void SetOrderButton(Button image)
        {
            _rectangleOnPanelButton = image;
            _view.SetButtonOnOrder(image);
        }

        public void SetOrderButtonCharacteristics(bool isActive) 
            => _view.SetButtonCharacteristics(isActive);

        public void SetPlanetPoint(PlanetPoint planetPoint)
        {
            if(_planetPoint != null)
                _planetPoint.CountSquadOnPoint--;
            planetPoint.CountSquadOnPoint++;
            _planetPoint = planetPoint;
            _presenter.UpdatePlanetPoint(planetPoint);
        }
        
        public void Select()
        {
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