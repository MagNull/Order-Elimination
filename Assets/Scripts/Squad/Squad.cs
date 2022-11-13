using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace OrderElimination
{
    public class Squad : MonoBehaviour, ISelectable, IMovable
    {
        private SquadInfo _squadInfo; 
        private SquadModel _model;
        private SquadView _view;
        private SquadPresenter _presenter;
        private PlanetPoint _planetPoint;
        private Order _order;
        private Button _orderOnPanelButton;
        public event Action<Squad> Selected;
        public event Action<Squad> Unselected;
        public PlanetPoint PlanetPoint => _planetPoint;
        public int AmountOfCharacters => _model.AmountOfCharacters;
        public IReadOnlyList<ISquadMember> Characters => _model.Characters;

        private void Awake()
        {
            _model = new SquadModel(new List<ISquadMember>());
            _view = new SquadView(transform);
            _presenter = new SquadPresenter(_model, _view, null);
            _planetPoint = null;
            _order = null;
        }

        public void AddCharacter(Character character) => _model.AddCharacter(character);

        public void RemoveCharacter(Character character) => _model.RemoveCharacter(character);

        public void Move(PlanetPoint planetPoint)
        {
            SetPlanetPoint(planetPoint);
            _model.Move(planetPoint);
        }

        public void SetOrder(Order order)
        {
            _order = order;
        }

        public void SetOrderButton(Button button)
        {
            _orderOnPanelButton = button;
            _view.SetButtonOnOrder(button);
        }

        public void SetOrderButtonCharacteristics(bool isActive) 
            => _view.SetButtonCharacteristics(isActive);

        public void SetPlanetPoint(PlanetPoint planetPoint)
        {
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
    }
}