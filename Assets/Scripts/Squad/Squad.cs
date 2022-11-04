using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace OrderElimination
{
    public class Squad : MonoBehaviour, ISelectable, IMovable
    {
        static public string LastSelectedSquadName;
        private SquadModel _model;
        private SquadView _view;
        private SquadPresenter _presenter;
        [ShowInInspector]
        private PlanetPoint _planetPoint;
        
        public PlanetPoint GetPlanetPoint()
        {
            return _planetPoint;
        }
            
        public event Action<Squad> Selected;
        public event Action<Squad> Unselected;

        public int AmountOfCharacters => _model.AmountOfCharacters;
        public IReadOnlyList<ISquadMember> Characters => _model.Characters;

        private void Awake()
        {
            _model = new SquadModel(new List<ISquadMember>());
            _view = new SquadView(transform);
            _presenter = new SquadPresenter(_model, _view, null);
            _planetPoint = null;
        }

        public void AddCharacter(Character character) => _model.AddCharacter(character);

        public void RemoveCharacter(Character character) => _model.RemoveCharacter(character);

        public void Move(PlanetPoint planetPoint)
        {
            SetPlanetPoint(planetPoint);
            _model.Move(planetPoint);
        }

        public void SetPlanetPoint(PlanetPoint planetPoint)
        {
            _planetPoint = planetPoint;
            _presenter.UpdatePlanetPoint(planetPoint);
        }

        public void Select()
        {
            Debug.Log("Select is done");
            Selected?.Invoke(this);
            _model.Select();
        }

        public void Unselect()
        {
            Debug.Log("Unselect is done");
            Selected?.Invoke(this);
            _model.Unselect();
        }

        // public void TargetIsSelected()
        // {
        //     PlanetPoint target = _planetPoint.GetSelectedPath();
        //     if(target != null && Squad.LastSelectedSquadName == gameObject.name)
        //     {
        //         SquadCommander.CreateResearchOrder(target, this);
        //         Move(target);
        //     }
        // }

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