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

        public int AmountOfCharacters => _model.AmountOfCharacters;
        public IReadOnlyList<ISquadMember> Characters => _model.Characters;

        private void Awake()
        {
            _model = new SquadModel(new List<ISquadMember>());
            _view = new SquadView();
            _presenter = new SquadPresenter(_model, _view);
        }

        public void AddCharacter(Character character) => _model.AddCharacter(character);

        public void RemoveCharacter(Character character) => _model.RemoveCharacter(character);

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