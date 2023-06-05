using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AI
{
    [CreateAssetMenu(fileName = "Character Behavior", menuName = "AI/Character Behavior")]
    public class CharacterBehavior : SerializedScriptableObject
    {
        public Selector BehaviorTreeRoot;

        public async UniTask Run(IBattleContext battleContext, AbilitySystemActor caster)
        {
            await BehaviorTreeRoot.Run(battleContext, caster);
        }
    }
}