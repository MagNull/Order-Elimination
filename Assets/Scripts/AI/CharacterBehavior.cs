using System;
using System.Collections.Generic;
using System.Linq;
using AI.Compositions;
using AI.Utils;
using Cysharp.Threading.Tasks;
using OrderElimination;
using OrderElimination.AbilitySystem;
using Sirenix.OdinInspector;
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

        [ShowInInspector]
        private Selector _behaviorTreeRoot;

        public async UniTask Run(IBattleContext battleContext, AbilitySystemActor caster)
        {
            AvoidObject = _avoidObject;
            var bb = new Blackboard();
            bb.Register("context", battleContext);
            bb.Register("caster", caster);
            //var evaluatedAbilities = caster
            //    .ActiveAbilities
            //    .Select(ability => new EvaluatedAbilityRunner(
            //        ability, 
            //        new AbilityImpact(ability.AbilityData, battleContext, caster, target.Position)))
            //bb.Register("evaluatedActiveAbilities", evaluatedAbilities);
            await _behaviorTreeRoot.TryRun(bb);
        }

        private void OnValidate()
        {
            if (_behaviorTreeRoot != null && nodes.Contains(_behaviorTreeRoot))
                return;

            _behaviorTreeRoot = nodes.FirstOrDefault(n => n is Selector) as Selector;
            if (_behaviorTreeRoot == null)
            {
                _behaviorTreeRoot = AddNode<Selector>();
            }

            _behaviorTreeRoot.name = "Root";
        }

        [Button, ShowIf("@_behaviorTreeRoot == null")]
        private void InitRoot()
        {
            if (_behaviorTreeRoot != null)
                return;
            _behaviorTreeRoot = AddNode<Selector>();
            _behaviorTreeRoot.name = "Root";
        }
    }
}