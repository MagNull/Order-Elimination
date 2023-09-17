using OrderElimination.Infrastructure.Maths;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class FilterFunctionCellSelector : ICellSelector
    {
        #region OdinVisuals
        private bool ValidateRangesSinCos() => ValidateRanges(new FloatRange(-1, 1));
        private bool ValidateRanges(FloatRange allowedValues)
        {
            for (var i = 0; i < AllowedRanges.Length; i++)
            {
                var isRangeInvalid =
                    AllowedRanges[i].Start < allowedValues.MinValue
                    || AllowedRanges[i].Start > allowedValues.MaxValue
                    || AllowedRanges[i].End < allowedValues.MinValue
                    || AllowedRanges[i].End > allowedValues.MaxValue;
                if (isRangeInvalid)
                    return false;
                //if (AllowedRanges[i].Start < allowedValues.MinValue)
                //    AllowedRanges[i].Start = allowedValues.MinValue;
                //if (AllowedRanges[i].Start > allowedValues.MaxValue)
                //    AllowedRanges[i].Start = allowedValues.MaxValue;
                //if (AllowedRanges[i].End < allowedValues.MinValue)
                //    AllowedRanges[i].End = allowedValues.MinValue;
                //if (AllowedRanges[i].End > allowedValues.MaxValue)
                //    AllowedRanges[i].End = allowedValues.MaxValue;
            }
            return true;
        }
        #endregion

        public enum FunctionOption
        {
            AngleCos
        }

        public enum AngleOption
        {
            TargetCasterCell,
            CasterTargetCell
        }

        [ShowInInspector, OdinSerialize]
        public FunctionOption Function { get; private set; }

        [ShowIf("@" + nameof(Function) + "==" + nameof(FunctionOption) + "." + nameof(FunctionOption.AngleCos))]
        [ShowInInspector, OdinSerialize]
        public AngleOption Angle { get; private set; }

        [ShowInInspector, OdinSerialize]
        public FloatRange[] AllowedRanges { get; private set; } = new[] { new FloatRange(-1, 1) };

        [ShowInInspector, OdinSerialize]
        public ICellSelector Source { get; private set; }

        public Vector2Int[] GetCellPositions(CellSelectorContext context)
        {
            var source = Source.GetCellPositions(context);
            var allowedCells = new List<Vector2Int>();
            if (Function == FunctionOption.AngleCos)
            {
                var caster = context.AskingEntity.Position;
                foreach (var cell in source)
                {
                    var isAllowed = false;
                    foreach (var target in context.SelectedCellPositions)
                    {
                        if (caster == target) 
                            continue;
                        Vector2 a;
                        Vector2 b;
                        switch (Angle)
                        {
                            case AngleOption.TargetCasterCell:
                                a = target - caster;
                                b = cell - caster;
                                break;
                            case AngleOption.CasterTargetCell:
                                a = caster - target;
                                b = cell - target;
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                        var angle = Vector2.Angle(a, b) * Mathf.Deg2Rad;
                        var cos = Mathf.Cos(angle);
                        if (AllowedRanges.Any(r => r.ContainsInclusive(cos)))
                            isAllowed = true;
                    }
                    if (isAllowed)
                        allowedCells.Add(cell);
                }
            }
            else
                throw new NotImplementedException();
            return allowedCells.ToArray();
        }
    }
}
