using System;
using System.Linq;
using AI.EditorGraph;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace AI
{
    public class AIRunner : MonoBehaviour
    {
        [SerializeField]
        private CharacterBehavior _behavior;

        private IBattleContext _context;
        private BattleLoopManager _battleLoopManager;

        [Inject]
        public void Construct(IBattleContext context, BattleLoopManager battleLoopManager)
        {
            _battleLoopManager = battleLoopManager;
            _context = context;
        }

        private void OnEnable()
        {
            _context.NewTurnStarted += OnTurnStarted;
        }
        
        private void OnDisable()
        {
            _context.NewTurnStarted -= OnTurnStarted;
        }

        private void OnTurnStarted(IBattleContext context)
        {
            if (context.ActiveSide != BattleSide.Enemies && context.ActiveSide != BattleSide.Others) 
                return;
            
            Run();
        }

        [Button]
        public async void Run()
        {
            var enemies = _context.EntitiesBank.GetEntities(BattleSide.Enemies);
            foreach (var enemy in enemies)
            {
                await _behavior.Run(_context, enemy);
                foreach (var activeAbilityRunner in enemy.ActiveAbilities)
                    activeAbilityRunner.AbilityData.TargetingSystem.CancelTargeting();
            }
            _battleLoopManager.StartNextTurn();
        }
    }
}