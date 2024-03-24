using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using OrderElimination.Utils;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

namespace OrderElimination.Debugging
{
    public class EntityKiller : MonoBehaviour
    {
        private IBattleContext _battleContext;
        private TextEmitter _textEmitter;
        private List<BattleSide> _allSides;

        [SerializeField]
        private BattleSide _sideToKill = BattleSide.Enemies;

        [SerializeField]
        private KeyCode _toggleSideShortcut = KeyCode.Equals;

        [SerializeField]
        private KeyCode _killShortcut = KeyCode.Minus;

        [Inject]
        private void Construct(IBattleContext battleContext, TextEmitter textEmitter)
        {
            _battleContext = battleContext;
            _textEmitter = textEmitter;
            _allSides = EnumExtensions.GetValues<BattleSide>().ToList();
        }

        [Button]
        public void KillAllEntities()
            => KillEntities(_sideToKill);

        private void KillEntities(BattleSide side)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            var killDamage = new DamageInfo(
                999999999, 1, 1, DamageType.Magic, LifeStatPriority.HealthFirst, null, false);
            var targetsToKill = _battleContext.EntitiesBank.GetActiveEntities(side);
            foreach (var entity in targetsToKill)
            {
                entity.TakeDamage(killDamage);
            }
#endif
        }

        private void Update()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (Input.GetKeyDown(_killShortcut))
            {
                KillAllEntities();
            }
            if (Input.GetKeyDown(_toggleSideShortcut))
            {
                var nextId = _allSides.IndexOf(_sideToKill);
                nextId = (nextId + 1) % _allSides.Count;
                _sideToKill = _allSides[nextId];
                _textEmitter.Emit($"{nameof(EntityKiller)} side set to {_sideToKill}");
            }
#endif
        }
    } 
}
