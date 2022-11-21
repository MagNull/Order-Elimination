using System.Linq;
using CharacterAbility;
using CharacterAbility.AbilityEffects;
using UnityEngine;
using VContainer;

namespace OrderElimination.BattleMap
{
    public class EnvironmentFactory
    {
        private readonly GameObject _viewPrefab;

        [Inject]
        public EnvironmentFactory(GameObject viewPrefab)
        {
            _viewPrefab = viewPrefab;
        }

        public EnvironmentObject Create(EnvironmentInfo environmentInfo)
        {
            var view = Object.Instantiate(_viewPrefab);
            view.GetComponent<SpriteRenderer>().sprite = environmentInfo.SpriteView;
            var result = new EnvironmentObject(environmentInfo.EnterEffects, view, environmentInfo.Stats,
                environmentInfo.IsWalkable);

            return result;
        }
    }
}