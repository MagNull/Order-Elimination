using System.Collections.Generic;
using UnityEngine;
using System;

namespace OrderElimination
{
    public class UnitPresenter
    {
        private readonly UnitModel _model;
        private readonly UnitView _view;
        public UnitPresenter(UnitModel model, UnitView view)
        {
            _model = model;
            _view = view;
        }

        public void Subscribe()
        {
            _model.Moved += _view.OnMove;
            _model.Selected += _view.OnSelect;
            _model.Unselected += _view.OnUnselect;
        }
        
        public void Unsubscribe()
        {
            _model.Moved -= _view.OnMove;
            _model.Selected -= _view.OnSelect;
            _model.Unselected -= _view.OnUnselect;
        }
    }
}