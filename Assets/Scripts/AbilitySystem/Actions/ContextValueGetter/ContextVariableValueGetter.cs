//using Sirenix.Serialization;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace OrderElimination.AbilitySystem
//{
//    //Cut out because doesn't support stacking and same variable names
//    public class ContextVariableValueGetter : IContextValueGetter
//    {
//        public string DisplayedFormula => $"@{VariableName}";

//        [OdinSerialize]
//        public string VariableName { get; private set; }//=randomVariableName

//        public IContextValueGetter Clone()
//        {
//            var clone = new ContextVariableValueGetter();
//            clone.VariableName = VariableName;
//            return clone;
//        }

//        public float GetValue(ValueCalculationContext context)
//        {
//            return context.ContextVariables[VariableName];
//        }
//    }
//}
