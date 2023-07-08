﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AI.EditorGraph;
using Cysharp.Threading.Tasks;
using OrderElimination;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using UnityEngine;
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

        private IBattleContext _context;
        private BattleLoopManager _battleLoopManager;

        [Inject]
        public void Construct(IBattleContext context, BattleLoopManager battleLoopManager)
        {
            _battleLoopManager = battleLoopManager;
            _context = context;
        }

        [Button]
        public async void Run(BattleSide playingSide)
        {
            var enemies = _context.EntitiesBank.GetEntities(playingSide);
            var templates =
                enemies
                    .Select(enemy =>
                        (_context.EntitiesBank.GetBattleCharacterData(enemy).CharacterData, enemy));
            templates = templates.OrderBy(el => el.CharacterData.Role);

            foreach (var enemyData in templates)
            {
                if (!_characterToBehaviors.ContainsKey(enemyData.CharacterData))
                    continue;
                
                var characterBehavior = _characterToBehaviors[enemyData.CharacterData];
                await characterBehavior.Run(_context, enemyData.enemy);
  
                foreach (var activeAbilityRunner in enemyData.enemy.ActiveAbilities)
                    activeAbilityRunner.AbilityData.TargetingSystem.CancelTargeting();
            }

            _battleLoopManager.StartNextTurn();
        }
    }
}