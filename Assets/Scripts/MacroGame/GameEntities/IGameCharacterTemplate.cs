using AI;
using OrderElimination.AbilitySystem;
using OrderElimination.MacroGame;
using UnityEngine;

namespace OrderElimination
{
    public interface IGameCharacterTemplate
    {
        public string Name { get; }
        public Sprite BattleIcon { get; }
        public Sprite Avatar { get; }
        public int Price { get; }
        public int Reward { get; }
        public Role Role { get; }

        public IReadOnlyGameCharacterStats GetBaseBattleStats();
        public ActiveAbilityBuilder[] GetActiveAbilities();
        public PassiveAbilityBuilder[] GetPassiveAbilities();
    } 
}
