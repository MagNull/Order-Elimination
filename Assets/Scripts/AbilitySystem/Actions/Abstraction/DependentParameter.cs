using OrderElimination.AbilitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    //TODO-КОНФИГУРАТОР Добавить кастомный редактор, чтобы можно было задавать ValueGetter формулой
    public class DependentParameter<TValue>// : DependentParameter<ActionUseContext, TValue>
    {
        public Func<ActionExecutionContext, TValue> ValueGetter { get; set; } = context => default;

        public TValue GetValue(ActionExecutionContext useContext) 
            => ValueGetter != null ? ValueGetter(useContext) : throw new InvalidOperationException();
    }

    //public abstract class DependentParameter<TContext, TValue>
    //{
    //    public Func<TContext, TValue> ValueGetter { get; protected set; }

    //    public TValue GetValue(TContext useContext)
    //        => ValueGetter != null ? ValueGetter(useContext) : throw new InvalidOperationException();

    //    public void SetValueGetter(Func<TContext, TValue> valueGetter) => ValueGetter = valueGetter;
    //}
}
