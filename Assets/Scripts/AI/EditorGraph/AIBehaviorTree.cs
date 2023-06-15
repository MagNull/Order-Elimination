using System.Collections.Generic;
using AI.Compositions;
using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem;
using OrderElimination.BM;
using UnityEngine;
using XNode;

namespace AI.EditorGraph
{
    [CreateAssetMenu]
    public class AIBehaviorTree : NodeGraph
    {
        public static IReadOnlyList<EnvironmentInfo> AvoidObject;

        [SerializeField]
        private List<EnvironmentInfo> _avoidObject;
        public Selector BehaviorTreeRoot;

        public async UniTask Run()
        {
            await BehaviorTreeRoot.Run(new Blackboard());
        }
    }
}