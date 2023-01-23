using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace OrderElimination.Battle
{
    [Serializable]
    public class CharactersBank : IReadOnlyCharacterBank
    {
        [ShowInInspector]
        private readonly List<BattleCharacter> _characters;

        public IReadOnlyList<BattleCharacter> Characters => _characters;
        
        public CharactersBank()
        {
            _characters = new List<BattleCharacter>();
        }

        public void AddCharacter(BattleCharacter battleCharacter) => _characters.Add(battleCharacter);

        public void RemoveCharacter(BattleCharacter battleCharacter) => _characters.Remove(battleCharacter);

        public void AddCharactersRange(IEnumerable<BattleCharacter> battleCharacters) =>
            _characters.AddRange(battleCharacters);

        public List<BattleCharacter> GetEnemies() => _characters.FindAll(x => x.Side == BattleObjectSide.Enemy);

        public List<BattleCharacter> GetAllies() => _characters.FindAll(x => x.Side == BattleObjectSide.Ally);
    }

    public interface IReadOnlyCharacterBank
    {
        public IReadOnlyList<BattleCharacter> Characters { get; }

        public List<BattleCharacter> GetEnemies();

        public List<BattleCharacter> GetAllies();
    }
}