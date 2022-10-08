using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination
{
    public class Squad : MonoBehaviour, ISelectable, IMovable
    {
        private SquadModel _model;
        private SquadView _view;
        private SquadPresenter _presenter;

        public int AmountOfUnits => _model.AmountOfUnits;
        public IReadOnlyList<Unit> Units => _model.Units;

        private void Awake()
        {
            _model = new SquadModel(new List<Unit>());
            _view = new SquadView();
            _presenter = new SquadPresenter(_model, _view);
        }

        public void AddUnit(Unit unit) => _model.AddUnit(unit);

        public void RemoveUnit(Unit unit) => _model.RemoveUnit(unit);

        public void Move(Vector2Int position) => _model.Move(position);

        public void Select() => _model.Select();
        
        public void Unselect() => _model.Unselect();

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