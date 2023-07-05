using OrderElimination.AbilitySystem;
using UnityEngine;

namespace OrderElimination
{
    public interface IBattleStructureTemplate
    {
        public string Name { get; }
        public Sprite BattleIcon { get; }
        public GameObject VisualModel { get; }
        public float MaxHealth { get; }
        public IBattleObstacleSetup ObstacleSetup { get; }

        public PassiveAbilityBuilder[] GetPossesedAbilities();
    } 
}
