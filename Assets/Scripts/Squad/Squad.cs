using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination
{
    public class Squad : MonoBehaviour, ISelectable, IMovable
    {
        static public string LastSelectedSquadName;
        private SquadModel _model;
        private SquadView _view;
        private SquadPresenter _presenter;
        
        [SerializeField] private PlanetPoint _planetPoint;

        public int AmountOfCharacters => _model.AmountOfCharacters;
        public IReadOnlyList<ISquadMember> Characters => _model.Characters;

        private void Awake()
        {
            _model = new SquadModel(new List<ISquadMember>());
            _view = new SquadView(transform);
            _presenter = new SquadPresenter(_model, _view, _planetPoint);
        }

        public void AddCharacter(Character character) => _model.AddCharacter(character);

        public void RemoveCharacter(Character character) => _model.RemoveCharacter(character);

        public void Move(PlanetPoint planetPoint)
        {
            _planetPoint = planetPoint;
            _presenter.Unsubscribe();
            _presenter.UpdatePlanetPoint(planetPoint);
            _presenter.Subscribe();
            _model.Move(planetPoint);
        }
        public void Select()
        {
            Squad.LastSelectedSquadName = gameObject.name;
            _model.Select();
        }

        public void Unselect()
        {
            _model.Unselect();
        }

        public void TargetIsSelected()
        {
            PlanetPoint target = _planetPoint.GetSelectedPath();
            Debug.Log(target);
            if(target != null && Squad.LastSelectedSquadName == gameObject.name)
            {
                SquadCommander.CreateResearchOrder(target, this);
                Move(target);
            }
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