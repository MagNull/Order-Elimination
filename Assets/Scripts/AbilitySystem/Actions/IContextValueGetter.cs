using OrderElimination.AbilitySystem;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OrderElimination.AbilitySystem
{
    public enum MathOperation
    {
        Add,
        Subtract,
        Multiply,
        Divide,
    }

    public interface IContextValueGetter
    {
        public string DisplayedFormula { get; }

        float GetValue(ActionContext useContext);
    }

    [Serializable]
    public struct ConstValueGetter : IContextValueGetter
    {
        [OdinSerialize]
        public float Value { get; private set; }

        public string DisplayedFormula => Value.ToString();

        public float GetValue(ActionContext useContext) => Value;
    }

    [Serializable]
    public struct MathValueGetter : IContextValueGetter
    {
        [OdinSerialize]
        public IContextValueGetter Left { get; private set; }

        [OdinSerialize]
        public MathOperation Operation { get; private set; }

        [OdinSerialize]
        public IContextValueGetter Right { get; private set; }

        public string DisplayedFormula
        {
            get
            {
                var leftValue = Left != null ? Left.DisplayedFormula : "_";
                var rightValue = Right != null ? Right.DisplayedFormula : "_";
                var operation = Operation switch
                {
                    MathOperation.Add => "+",
                    MathOperation.Subtract => "-",
                    MathOperation.Multiply => "*",
                    MathOperation.Divide => "/",
                    _ => throw new NotImplementedException(),
                };
                return $"({leftValue} {operation} {rightValue})";
            }
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
        [OdinSerialize]
        public BattleStat CasterStat { get; private set; }

        [OdinSerialize]
        public bool UseUnmodifiedValue { get; private set; }

        public string DisplayedFormula => $"Caster.{CasterStat}({(UseUnmodifiedValue ? "orig" : "mod")})";

        public float GetValue(ActionContext useContext)
        {
            if (useContext.ActionMaker == null
                || !useContext.ActionMaker.BattleStats.HasParameter(CasterStat))
                return 0;
            if (!UseUnmodifiedValue)
                return useContext.ActionMaker.BattleStats.GetParameter(CasterStat).ModifiedValue;
            else
                return useContext.ActionMaker.BattleStats.GetParameter(CasterStat).UnmodifiedValue;
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

        public float GetValue(ActionContext useContext)
        {
            if (useContext.ActionTarget == null
                || !useContext.ActionTarget.BattleStats.HasParameter(TargetStat))
                return 0;
            if (!UseUnmodifiedValue)
                return useContext.ActionTarget.BattleStats.GetParameter(TargetStat).ModifiedValue;
            else
                return useContext.ActionTarget.BattleStats.GetParameter(TargetStat).UnmodifiedValue;
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
                var start = RangeStart != null ? RangeStart.DisplayedFormula : "_";
                var end = RangeEnd != null ? RangeEnd.DisplayedFormula : "_";
                return $"[{start};{end}]";
            }
        }
        public float GetValue(ActionContext useContext)
        {
            var rand = Random.Range(RangeStart.GetValue(useContext), RangeEnd.GetValue(useContext));
            return RoundToInt ? Mathf.RoundToInt(rand) : rand;
        }
    }
}
