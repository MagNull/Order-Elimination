using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination
{
    public class Character : MonoBehaviour, ISquadMember
    {
        private CharacterModel _model;
        private CharacterView _view;
        private CharacterPresenter _presenter;
        public int Rang => _model.GetStats();

        private void Awake() 
        {
            _model = new CharacterModel();
            _view = new CharacterView();
            _presenter = new CharacterPresenter(_model, _view);    
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

        public int GetStats() =>_model.GetStats();
    }
}