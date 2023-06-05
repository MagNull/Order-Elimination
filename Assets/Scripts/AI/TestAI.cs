using System.Linq;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace AI
{
    public class TestAI : MonoBehaviour
    {
        [SerializeField]
        private CharacterBehavior _behavior;

        private IBattleContext _context;

        [Inject]
        public void Construct(IBattleContext context)
        {
            _context = context;
        }
        
        [Button]
        public void Test()
        {
            var enemy = _context.EntitiesBank.GetEntities(BattleSide.Enemies).First();
            _behavior.Run(_context, enemy);
        }
    }
}