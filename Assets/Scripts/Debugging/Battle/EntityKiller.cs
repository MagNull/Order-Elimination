using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace OrderElimination.Debugging
{
    public class EntityKiller : MonoBehaviour
    {
        private IBattleContext _battleContext;

        [SerializeField]
        private BattleSide _sideToKill = BattleSide.Enemies;

        [Inject]
        private void Construct(IBattleContext battleContext)
        {
            _battleContext = battleContext;
        }

        [Button]
        public void KillAllEntities()
        {
            var killDamage = new DamageInfo(
                999999999, 1, 1, DamageType.Magic, LifeStatPriority.HealthFirst, null, false);
            var targetsToKill = _battleContext.EntitiesBank.GetActiveEntities(_sideToKill);
            foreach (var enemy in targetsToKill)
            {
                enemy.TakeDamage(killDamage);
            }
        }
    } 
}
