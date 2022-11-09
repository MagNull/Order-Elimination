using OrderElimination;

namespace CharacterAbility.AbilityEffects
{
    public class AreaAbility : Ability
    {
        private readonly int _radius;
        private readonly Ability _areaAbility;

        public AreaAbility(IAbilityCaster caster, Ability areaAbility, int radius) : base(caster)
        {
            _radius = radius;
            _areaAbility = areaAbility;
        }
        
        public override void Use(IBattleObject target, IReadOnlyBattleStats stats, BattleMap battleMap)
        {
            var targets = battleMap.GetBattleObjectsInRadius(target, _radius);
            targets.Remove(target);
            var aoeStat = new BattleStats(stats)
            {
                Accuracy = 100
            };
            foreach (var battleObject in targets)
            {
                _areaAbility.Use(battleObject, aoeStat, battleMap);
            }
        }
    }
}