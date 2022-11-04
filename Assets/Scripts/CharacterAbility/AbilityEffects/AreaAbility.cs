namespace CharacterAbility.AbilityEffects
{
    public class AreaAbility : Ability
    {
        private readonly int _radius;
        private readonly Ability _areaAbility;

        public AreaAbility(BattleCharacter caster, Ability areaAbility, int radius) : base(caster)
        {
            _radius = radius;
            _areaAbility = areaAbility;
        }

        public override void Use(IBattleObject target, BattleMap battleMap)
        {
            var targets = battleMap.GetBattleObjectsInRadius(target, _radius);
            foreach (var battleObject in targets)
            {
                _areaAbility.Use(battleObject, battleMap);
            }
        }
    }
}