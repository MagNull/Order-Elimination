using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;

namespace OrderElimination
{
    [Serializable]
    public struct ValueModifier
    {
        public ValueModifier(BinaryMathOperation operation, float operand)
        {
            Operation = operation;
            Operand = operand;
        }

        [HideLabel]
        [HorizontalGroup]
        [ShowInInspector]
        [OdinSerialize]
        public BinaryMathOperation Operation { get; set; }

        [HideLabel]
        [HorizontalGroup]
        [ShowInInspector]
        [OdinSerialize]
        public float Operand { get; set; }

        public float ModifyValue(float value)
            => MathExtensions.PerformBinaryOperation(value, Operand, Operation);
    }
}