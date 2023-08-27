using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace OrderElimination.Debugging
{
    public class EnemyKiller : MonoBehaviour
    {
        private IBattleContext _battleContext;

        [Inject]
        private void Construct(IBattleContext battleContext)
        {
            _battleContext = battleContext;
        }

        [Button]
        public void KillAllEnemies()
        {
            var killDamage = new DamageInfo(
                999999999, 1, 1, DamageType.Magic, LifeStatPriority.HealthFirst, null);
            var targetsToKill = _battleContext.EntitiesBank.GetActiveEntities(BattleSide.Enemies);
            foreach (var enemy in targetsToKill)
            {
                enemy.TakeDamage(killDamage);
            }
        }
    } 
}
