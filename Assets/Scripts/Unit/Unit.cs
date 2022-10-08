using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination
{
    public class Unit : MonoBehaviour, ISelectable, IMovable
    {
        private UnitModel _model;
        private UnitView _view;
        private UnitPresenter _presenter;
        public int Rang => _model.rang;

        private void Awake() 
        {
            _model = new UnitModel();
            _view = new UnitView();
            _presenter = new UnitPresenter(_model, _view);    
        }

        public void Move(Vector2Int position) => _model.Move(position);

        public void Select() => _model.Select();

        public void Unselect() => _model.Unselect();

        public void RaiseExpirience(float expirience) => _model.RaiseExpirience(expirience);

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