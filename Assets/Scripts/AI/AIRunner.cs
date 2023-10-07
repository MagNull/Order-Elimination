using System.Collections.Generic;
using System.Linq;
using AI.EditorGraph;
using Cysharp.Threading.Tasks;
using OrderElimination;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

namespace AI
{
    public enum Role
    {
        Specialists,
        Vanguard,
        Monsters,
        Shooters,
        Wizards
    }

    public class AIRunner : SerializedMonoBehaviour
    {
        [SerializeField]
        private Dictionary<IGameCharacterTemplate, CharacterBehavior> _characterToBehaviors = new();

        [SerializeField]
        private CharacterBehavior _generalBehavior;

        private IBattleContext _context;

        [Inject]
        public void Construct(IBattleContext context)
        {
            _context = context;
        }

        [Button]
        public async UniTask Run(BattleSide playingSide)
        {
            var enemies = _context.EntitiesBank.GetActiveEntities(playingSide);
            var templates =
                enemies
                    .Select(enemy =>
                        (_context.EntitiesBank.GetBasedCharacter(enemy).CharacterData, enemy));
            templates = templates.OrderBy(el => el.CharacterData.Role);

            foreach (var enemyData in templates)
            {
                if (enemyData.enemy.IsDisposedFromBattle)
                    continue;
                var characterBehavior = _characterToBehaviors.TryGetValue(enemyData.CharacterData, out var behavior) ? 
                    behavior : _generalBehavior;
                await characterBehavior.Run(_context, enemyData.enemy);
  
                foreach (var activeAbilityRunner in enemyData.enemy.ActiveAbilities)
                    activeAbilityRunner.AbilityData.TargetingSystem.CancelTargeting();
            }
        }
    }
}