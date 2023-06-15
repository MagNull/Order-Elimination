using System.Collections.Generic;
using System.Linq;
using AI.Utils;
using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem;
using UnityEngine;

namespace AI.Actions
{
    public class UseAbility : BehaviorTreeTask
    {
        [SerializeField]
        private ActiveAbilityBuilder _ability;
        
        public override async UniTask<bool> Run(Blackboard blackboard)
        {
            var targets = blackboard.Get<IEnumerable<AbilitySystemActor>>("targets");
            var context = blackboard.Get<IBattleContext>("context");
            var caster = blackboard.Get<AbilitySystemActor>("caster");
            
            var abilities = caster.ActiveAbilities;
            var ability = abilities.FirstOrDefault(ab => ab.AbilityData.BasedBuilder == _ability);
            if (ability == null)
                return false;
            
            switch (ability.AbilityData.TargetingSystem)
            {
                case SingleTargetTargetingSystem:
                {
                    foreach (var target in targets)
                    {
                        var result = await ability.CastSingleTarget(context, caster, target.Position);
                        return result;
                    }

                    return false;
                }
                case NoTargetTargetingSystem:
                {
                    if (!ability.InitiateCast(context, caster))
                        return false;
                    ability.AbilityData.TargetingSystem.ConfirmTargeting();
                    return true;
                }
            }

            

            return false;
        }
    }
}