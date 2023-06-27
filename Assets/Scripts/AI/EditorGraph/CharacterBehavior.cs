using System.Collections.Generic;
using AI.Compositions;
using Cysharp.Threading.Tasks;
using OrderElimination;
using OrderElimination.AbilitySystem;
using UnityEngine;
using XNode;

namespace AI.EditorGraph
{
    [CreateAssetMenu]
    public class CharacterBehavior : NodeGraph
    {
        public static IReadOnlyList<StructureTemplate> AvoidObject;

        [SerializeField]
        private List<StructureTemplate> _avoidObject;

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