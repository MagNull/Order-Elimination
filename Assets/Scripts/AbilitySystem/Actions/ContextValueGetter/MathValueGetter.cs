using OrderElimination.Infrastructure;
using Sirenix.Serialization;
using System;

namespace OrderElimination.AbilitySystem
{
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
}
