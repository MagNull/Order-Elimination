using UnityEngine;
using UnityEngine.UI;

namespace CharacterAbility
{
    public class AbilityView
    {
        [SerializeField]
        private readonly Sprite _abilityIcon;
        private readonly Ability _ability;
        private readonly BattleMapView _battleMapView;

        public AbilityView(Ability ability, AbilityInfo info, BattleMapView battleMapView)
        {
            _ability = ability;
            _battleMapView = battleMapView;
            _abilityIcon = info.Icon;
        }

        public Sprite AbilityIcon => _abilityIcon;

        public void OnUsed()
        {
            
        }

        public void Clicked()
        {
            _ability.Use(null, _battleMapView);
        }
    }
}