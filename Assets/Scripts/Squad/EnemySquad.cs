using System.Collections.Generic;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using VContainer;

namespace OrderElimination
{
    public class EnemySquad : SerializedMonoBehaviour, ISquad
    {
        [OdinSerialize]
        [ShowInInspector]
        private List<Character> _testSquadMembers;
        private SquadModel _model;
        private Button _rectangleOnPanelButton;
        private CharactersMediator _charactersMediator;
        public PlanetPoint PlanetPoint { get; private set; }
        public int AmountOfCharacters => _model.AmountOfMembers;
        public IReadOnlyList<Character> Members => _model.Members;
        public bool AlreadyMove = false;


        [Inject]
        private void Construct(CharactersMediator charactersMediator)
        {
            _charactersMediator = charactersMediator;
        }

        private void Awake()
        {
            _model = new SquadModel(_testSquadMembers);
        }

        public void Add(Character member) => _model.Add(member);

        public void Remove(Character member) => _model.RemoveCharacter(member);

        public void Move(PlanetPoint planetPoint)
        {
            AlreadyMove = true;
            PlanetPoint?.RemoveSquad();
            planetPoint?.AddSquad();
            SetPlanetPoint(planetPoint);
            _model.Move(planetPoint);
        }

        private void SetPlanetPoint(PlanetPoint planetPoint)
        {
            PlanetPoint = planetPoint;
        }
    }
}