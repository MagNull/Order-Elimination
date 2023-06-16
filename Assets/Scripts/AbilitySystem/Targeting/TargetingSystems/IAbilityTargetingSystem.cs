using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public bool StartTargeting(IBattleContext context, AbilitySystemActor caster);
        public bool ConfirmTargeting();
        public bool CancelTargeting();
        public CellGroupsContainer ExtractCastTargetGroups();
    }

    public interface IRequireSelectionTargetingSystem : IAbilityTargetingSystem
    {
        public IEnumerable<Vector2Int> CurrentAvailableCells { get; }
        public IEnumerable<Vector2Int> SelectedCells { get; }
        //No point to show conditions since they can change during targeting

        public event Action<IRequireSelectionTargetingSystem> ConfirmationUnlocked;
        public event Action<IRequireSelectionTargetingSystem> ConfirmationLocked;
        public event Action<IRequireSelectionTargetingSystem> SelectionUpdated;
        public event Action<IRequireSelectionTargetingSystem> AvailableCellsUpdated;

        //ToBeRemoved
        public bool SetAvailableCellsForSelection(Vector2Int[] availableCellsForSelection);
    }
}
