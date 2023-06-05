using System.Linq;
using OrderElimination.AbilitySystem;

namespace AI.Utils
{
    public struct AbilityImpact
    {
        public float Damage;
        public float Heal;
        
        private readonly IActiveAbilityData _data;
        private readonly IBattleContext _battleContext;
        private readonly AbilitySystemActor _caster;
        private readonly AbilitySystemActor _target;

        public AbilityImpact(IActiveAbilityData data, IBattleContext battleContext, AbilitySystemActor caster,
            AbilitySystemActor target)
        {
            _data = data;
            _battleContext = battleContext;
            _caster = caster;
            _target = target;
            Damage = 0;
            Heal = 0;
            Process();
        }

        private void Process()
        {
            var actions = _data.Execution.ActionInstructions.Select(i => i.Action);
            var actionContext = new ActionContext(_battleContext, _data.TargetingSystem.ExtractCastTargetGroups(),
                _caster, _target);
            foreach (var battleAction in actions)
            {
                switch (battleAction)
                {
                    case InflictDamageAction inflictDamageAction:
                        Damage += inflictDamageAction.DamageSize.GetValue(actionContext);
                        break;
                    case HealAction healAction:
                        Heal += healAction.HealSize.GetValue(actionContext);
                        break;
                }
            }
        }
    }
}