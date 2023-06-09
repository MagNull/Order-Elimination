using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem;
using OrderElimination.BM;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AI
{
    [CreateAssetMenu(fileName = "Character Behavior", menuName = "AI/Character Behavior")]
    public class CharacterBehavior : SerializedScriptableObject
    {
        public static IReadOnlyList<EnvironmentInfo> AvoidObject;

        [SerializeField]
        private List<EnvironmentInfo> _avoidObject;
        public Selector BehaviorTreeRoot;

        public async UniTask Run(IBattleContext battleContext, AbilitySystemActor caster)
        {
            AvoidObject = _avoidObject;
            var bb = new Blackboard();
            bb.Register("context", battleContext);
            bb.Register("caster", caster);
            await BehaviorTreeRoot.Run(bb);
        }
    }
}