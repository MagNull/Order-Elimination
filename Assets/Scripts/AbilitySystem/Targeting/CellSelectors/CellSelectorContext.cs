﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class CellSelectorContext
    {
        public IBattleContext BattleContext { get; }
        public AbilitySystemActor AskingEntity { get; }
        public Vector2Int[] SelectedCellPositions { get; }

        public CellSelectorContext(
            IBattleContext battleContext,
            AbilitySystemActor askingEntity, 
            params Vector2Int[] selectedCellPositions)
        {
            AskingEntity = askingEntity;
            SelectedCellPositions = selectedCellPositions;
            BattleContext = battleContext;
        }
    }
}
