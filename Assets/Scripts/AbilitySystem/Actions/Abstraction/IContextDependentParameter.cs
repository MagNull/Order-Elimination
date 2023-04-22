using OrderElimination.AbilitySystem;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    //TODO-КОНФИГУРАТОР Добавить кастомный редактор, чтобы можно было задавать ValueGetter формулой
    public interface IContextDependentParameter<TValue>
    {
        public Expression<Func<ActionExecutionContext, TValue>> ValueGetterExpression { get; }
        public TValue GetValue(ActionExecutionContext useContext);
    }

    [Serializable]
    public struct FloatDependentParameter : IContextDependentParameter<float>
    {
        [SerializeField]
        private Expression<Func<ActionExecutionContext, float>> _valueGetterExpression;
        [SerializeField]
        private Func<ActionExecutionContext, float> _compiledValueGetter;

        public Expression<Func<ActionExecutionContext, float>> ValueGetterExpression => _valueGetterExpression;

        public float GetValue(ActionExecutionContext useContext)
            => _compiledValueGetter != null ? _compiledValueGetter(useContext) : throw new InvalidOperationException();

        public FloatDependentParameter(Expression<Func<ActionExecutionContext, float>> valueGetterExpression)
        {
            _valueGetterExpression = valueGetterExpression;
            _compiledValueGetter = _valueGetterExpression.Compile();
        }
    }
}
