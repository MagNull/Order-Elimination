using System;
using System.Collections.Generic;

namespace OrderElimination 
{
    public class SquadModel : ISelectable, IMovable
    {
        private List<ISquadMember> _characters;
        private int _rang;
        public int AmountOfCharacters => _characters.Count;
        public IReadOnlyList<ISquadMember> Characters => _characters;

        public event Action<PlanetPoint> Moved;
        public event Action Selected;
        public event Action Unselected;

        public SquadModel(List<ISquadMember> characters)
        {
            if(characters.Count != 0)
            {
                _characters = characters;
                _rang = 0;
                foreach (var character in _characters)
                {
                    _rang += character.GetStats();
                }
                _rang /= AmountOfCharacters;
            }
        }

        public void AddCharacter(Character character) => _characters.Add(character);

        public void RemoveCharacter(Character character)
        {
            if (!_characters.Contains(character))
                throw new ArgumentException("No such character in squad");
            _characters.Remove(character);
        }

        public void DistributeExpirience(float expirience)
        {
            foreach (var character in _characters)
            {
                character.RaiseExpirience(expirience / AmountOfCharacters);
            }
        }
        
        public void Move(PlanetPoint planetPoint)
        {
            // foreach(var character in _characters)
            // {
            //     character.Move(planetPoint);
            // }
            Moved?.Invoke(planetPoint);
        }

        public void Select() => Selected?.Invoke();   

        public void Unselect() => Unselected?.Invoke();
    }
}
