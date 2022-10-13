using System.Collections.Generic;
using UnityEngine;
using System;

namespace OrderElimination
{
    public class CharacterPresenter
    {
        private readonly CharacterModel _model;
        private readonly CharacterView _view;
        public CharacterPresenter(CharacterModel model, CharacterView view)
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