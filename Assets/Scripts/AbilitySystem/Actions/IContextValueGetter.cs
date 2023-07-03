﻿using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OrderElimination.AbilitySystem
{
    public interface IContextValueGetter : ICloneable<IContextValueGetter>
    {
        public const string EmptyValueReplacement = "_";

        public string DisplayedFormula { get; }

        float GetValue(ActionContext useContext);
    }

    [Serializable]
    public struct ConstValueGetter : IContextValueGetter
    {
        [OdinSerialize]
        public float Value { get; set; }

        public string DisplayedFormula => Value.ToString();

        public IContextValueGetter Clone()
        {
            var clone = new ConstValueGetter();
            clone.Value = Value;
            return clone;
        }

        public float GetValue(ActionContext useContext) => Value;

        public ConstValueGetter(float value) => Value = value;
    }

    [Serializable]
    public struct MathValueGetter : IContextValueGetter
    {
        [OdinSerialize]
        public IContextValueGetter Left { get; set; }

        [OdinSerialize]
        public MathOperation Operation { get; set; }

        [OdinSerialize]
        public IContextValueGetter Right { get; set; }

        public string DisplayedFormula
        {
            get
            {
                var leftValue = Left != null 
                    ? Left.DisplayedFormula 
                    : IContextValueGetter.EmptyValueReplacement;
                var rightValue = Right != null 
                    ? Right.DisplayedFormula 
                    : IContextValueGetter.EmptyValueReplacement;
                var operation = Operation.AsString();
                return $"({leftValue} {operation} {rightValue})";
            }
        }

        public IContextValueGetter Clone()
        {
            var clone = new MathValueGetter();
            clone.Left = Left.Clone();
            clone.Right = Right.Clone();
            clone.Operation = Operation;
            return clone;
        }

        public float GetValue(ActionContext useContext)
        {
            var leftValue = Left != null ? Left.GetValue(useContext) : 0;
            var rightValue = Right != null ? Right.GetValue(useContext) : 0;
            var result = Operation switch
            {
                MathOperation.Add => leftValue + rightValue,
                MathOperation.Subtract => leftValue - rightValue,
                MathOperation.Multiply => leftValue * rightValue,
                MathOperation.Divide => leftValue / rightValue,
                _ => throw new NotImplementedException(),
            };
            return result;
        }
    }

    [Serializable]
    public struct CasterStatGetter : IContextValueGetter
    {
        public CasterStatGetter(BattleStat stat, bool useUnmodified = false)
        {
            CasterStat = stat;
            UseUnmodifiedValue = useUnmodified;
        }

        [OdinSerialize]
        public BattleStat CasterStat { get; private set; }

        [OdinSerialize]
        public bool UseUnmodifiedValue { get; private set; }

        public string DisplayedFormula => $"Caster.{CasterStat}({(UseUnmodifiedValue ? "orig" : "mod")})";

        public IContextValueGetter Clone()
        {
            var clone = new CasterStatGetter();
            clone.CasterStat = CasterStat;
            clone.UseUnmodifiedValue = UseUnmodifiedValue;
            return clone;
        }

        public float GetValue(ActionContext useContext)
        {
            if (useContext.ActionMaker == null
                || !useContext.ActionMaker.BattleStats.HasParameter(CasterStat))
                return 0;
            if (!UseUnmodifiedValue)
                return useContext.ActionMaker.BattleStats[CasterStat].ModifiedValue;
            else
                return useContext.ActionMaker.BattleStats[CasterStat].UnmodifiedValue;
        }
    }

    [Serializable]
    public struct TargetStatGetter : IContextValueGetter
    {
        [OdinSerialize]
        public BattleStat TargetStat { get; private set; }

        [OdinSerialize]
        public bool UseUnmodifiedValue { get; private set; }

        public string DisplayedFormula => $"Caster.{TargetStat}({(UseUnmodifiedValue ? "orig" : "mod")})";

        public IContextValueGetter Clone()
        {
            var clone = new TargetStatGetter();
            clone.TargetStat = TargetStat;
            clone.UseUnmodifiedValue = UseUnmodifiedValue;
            return clone;
        }

        public float GetValue(ActionContext useContext)
        {
            if (useContext.ActionTarget == null
                || !useContext.ActionTarget.BattleStats.HasParameter(TargetStat))
                return 0;
            if (!UseUnmodifiedValue)
                return useContext.ActionTarget.BattleStats[TargetStat].ModifiedValue;
            else
                return useContext.ActionTarget.BattleStats[TargetStat].UnmodifiedValue;
        }
    }

    [Serializable]
    public struct RandomValueGetter : IContextValueGetter
    {
        [OdinSerialize]
        public IContextValueGetter RangeStart { get; private set; }

        [OdinSerialize]
        public IContextValueGetter RangeEnd { get; private set; }

        [OdinSerialize]
        public bool RoundToInt { get; private set; }

        public string DisplayedFormula
        {
            get
            {
                var start = RangeStart != null 
                    ? RangeStart.DisplayedFormula 
                    : IContextValueGetter.EmptyValueReplacement;
                var end = RangeEnd != null 
                    ? RangeEnd.DisplayedFormula 
                    : IContextValueGetter.EmptyValueReplacement;
                return $"[{start};{end}]";
            }
        }

        public IContextValueGetter Clone()
        {
            var clone = new RandomValueGetter();
            clone.RangeStart = RangeStart.Clone();
            clone.RangeEnd = RangeEnd.Clone();
            clone.RoundToInt = RoundToInt;
            return clone;
        }

        public float GetValue(ActionContext useContext)
        {
            var rand = Random.Range(RangeStart.GetValue(useContext), RangeEnd.GetValue(useContext));
            return RoundToInt ? Mathf.RoundToInt(rand) : rand;
        }
    }

    [Serializable]
    public struct EntitiesCountGetter : IContextValueGetter
    {
        [OdinSerialize]
        public IEntityCondition[] EntityConditions { get; private set; }

        [OdinSerialize]
        public IPointRelativePattern PatternToCheck { get; private set; }

        [OdinSerialize]
        public ActionEntity RelativeTo { get; private set; }

        public string DisplayedFormula => "EntitiesCount";

        public IContextValueGetter Clone()
        {
            var clone = new EntitiesCountGetter();
            clone.EntityConditions = CloneableCollectionsExtensions.Clone(EntityConditions);
            clone.PatternToCheck = PatternToCheck.Clone();
            clone.RelativeTo = RelativeTo;
            return clone;
        }

        public float GetValue(ActionContext useContext)
        {
            var entitiesCount = 0;
            var conditions = EntityConditions;
            var battleContext = useContext.BattleContext;
            var map = battleContext.BattleMap;
            var target = RelativeTo switch
            {
                ActionEntity.Caster => useContext.ActionMaker,
                ActionEntity.Target => useContext.ActionTarget,
                _ => throw new NotImplementedException(),
            };
            var points = PatternToCheck.GetAbsolutePositions(target.Position);
            foreach (var pos in points.Where(p => map.CellRangeBorders.Contains(p)))
            {
                entitiesCount += map.GetContainedEntities(pos)
                    .Where(e => IsEntityAllowed(e)).Count();
            }
            return entitiesCount;

            bool IsEntityAllowed(AbilitySystemActor entity)
                => conditions.All(c => c.IsConditionMet(battleContext, useContext.ActionMaker, entity));
        }
    }
}
