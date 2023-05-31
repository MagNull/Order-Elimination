using OrderElimination.AbilitySystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AI
{
    [CreateAssetMenu(fileName = "Character Behavior", menuName = "AI/Character Behavior")]
    public class CharacterBehavior : SerializedScriptableObject
    {
        public IBehaviorTreeTask BehaviorTreeRoot;

        public void Run(IBattleContext battleContext, AbilitySystemActor caster)
        {
            BehaviorTreeRoot.Run(battleContext, caster);
        }
    }
}