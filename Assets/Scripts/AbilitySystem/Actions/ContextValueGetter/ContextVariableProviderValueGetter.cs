//using Sirenix.OdinInspector;
//using Sirenix.OdinInspector.Editor;
//using Sirenix.Serialization;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//namespace OrderElimination.AbilitySystem
//{
//    //Cut out because doesn't support stacking and same variable names
//    public class ContextVariableProviderValueGetter : IContextValueGetter
//    {
//        [Obsolete("Should only be used via Odin in inspector.")]
//        public ContextVariableProviderValueGetter()//Odin-only
//        {
//            VariableSuccessor = new ConstValueGetter(float.NaN);
//            VariableProviderAsValueGetter = new ConstValueGetter(float.NaN);
//            VariableProvider = VariableProviderAsValueGetter.GetValue;
//        }

//        public ContextVariableProviderValueGetter(
//            string variableName,
//            IContextValueGetter variableSuccessor,
//            Func<ValueCalculationContext, float> variableProvider)
//        {
//            VariableName = variableName;
//            VariableSuccessor = variableSuccessor;
//            VariableProvider = variableProvider;
//        }

//        public ContextVariableProviderValueGetter(
//            string variableName,
//            IContextValueGetter variableSuccessor,
//            IContextValueGetter variableProvider)
//        {
//            VariableName = variableName;
//            VariableSuccessor = variableSuccessor;
//            _variableProviderAsValueGetter = variableProvider;
//            VariableProvider = variableProvider.GetValue;
//        }

//        public string DisplayedFormula
//            => $"({VariableSuccessor.DisplayedFormula} as {VariableName})";

//        [OdinSerialize]
//        public string VariableName { get; private set; }

//        [HideInInspector, OdinSerialize]
//        private IContextValueGetter _variableProviderAsValueGetter;
//        [ShowInInspector, LabelText("Variable Provider")]
//        private IContextValueGetter VariableProviderAsValueGetter
//        {
//            get => _variableProviderAsValueGetter;
//            set
//            {
//                _variableProviderAsValueGetter = value;
//                VariableProvider = _variableProviderAsValueGetter.GetValue;
//            }
//        }//Odin-only

//        public Func<ValueCalculationContext, float> VariableProvider { get; private set; }

//        [OdinSerialize]
//        public IContextValueGetter VariableSuccessor { get; private set; }

//        public IContextValueGetter Clone()
//        {
//            var clone = new ContextVariableProviderValueGetter(
//                VariableName, VariableSuccessor.Clone(), VariableProvider);
//            clone._variableProviderAsValueGetter = _variableProviderAsValueGetter.Clone();
//            return clone;
//        }

//        public float GetValue(ValueCalculationContext context)
//        {
//            context.WriteVariable(VariableName, VariableProvider(context));
//            return VariableSuccessor.GetValue(context);
//        }
//    }
//}
