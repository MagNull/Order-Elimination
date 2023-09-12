using System;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public interface IAbilityTargetingSystem
    {
        public bool IsTargeting { get; }
        public bool IsConfirmed { get; }
        public bool IsConfirmAvailable { get; }

        public event Action<IAbilityTargetingSystem> TargetingStarted;
        public event Action<IAbilityTargetingSystem> TargetingConfirmed;
        public event Action<IAbilityTargetingSystem> TargetingCanceled;

        public bool StartTargeting(IBattleContext battleContext, AbilitySystemActor caster);
        public bool ConfirmTargeting();
        public bool CancelTargeting();
        public CellGroupsContainer ExtractCastTargetGroups();
    }

    public interface IRequireSelectionTargetingSystem : IAbilityTargetingSystem
    {
        //Remove because unsafe if until start? In that case Peek...() needs to be optimized.
        public IEnumerable<Vector2Int> CurrentAvailableCells { get; }
        public IEnumerable<Vector2Int> SelectedCells { get; }
        public int NecessaryTargetsLeft { get; }
        //No point to show conditions since they can change during targeting
        //public IEnumerable<ICellCondition> CurrentCellsConditions { get; }

        public event Action<IRequireSelectionTargetingSystem> ConfirmationUnlocked;
        public event Action<IRequireSelectionTargetingSystem> ConfirmationLocked;
        public event Action<IRequireSelectionTargetingSystem> SelectionUpdated;
        public event Action<IRequireSelectionTargetingSystem> AvailableCellsUpdated;

        /// <summary>
        /// Returns positions that can be targeted by specified caster in current conditions. 
        /// Does not require targeting system to be started first.
        /// </summary>
        /// <param name="battleContext"></param>
        /// <param name="caster"></param>
        /// <returns></returns>
        public Vector2Int[] PeekAvailableCells(IBattleContext battleContext, AbilitySystemActor caster);
        /// <summary>
        /// Returns if position can be targeted by specified caster in current conditions. 
        /// Does not require targeting system to be started first.
        /// </summary>
        /// <param name="battleContext"></param>
        /// <param name="caster"></param>
        /// <returns></returns>
        public bool CanSelectPeek(IBattleContext battleContext, AbilitySystemActor caster, Vector2Int cellPosition);
        public bool TryPeekDistribution(
            out CellGroupsContainer cellGroups, 
            IBattleContext battleContext, AbilitySystemActor caster, params Vector2Int[] selectedPositions);
        public bool Select(Vector2Int cellPosition);
        public bool Deselect(Vector2Int cellPosition);
    }
}
