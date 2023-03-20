using OrderElimination.AbilitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public enum EffectCharacter
    {
        Positive,
        Negative,
        Neutral
    }

    public interface IEffect
    {
        public static bool IsStackable { get; }
        public EffectCharacter EffectCharacter { get; }
        public IBattleAction[] ActionsOnApply { get; }

        public event Action<IEffect> Removed;//Destroyed Disposed Finished
    }

    public interface ITemporaryEffect : IEffect
    {
        public int Duration { get; }
        public event Action<ITemporaryEffect> EffectEnded;
    }

    //public interface IActionProcessingEffect : IEffect
    //{
    //    public IBattleAction[] ProcessIncomingActions(IBattleAction[] originalActions);
    //    public IBattleAction[] ProcessOutcomingActions(IBattleAction[] originalActions);
    //}

    //При таком варианте мы сразу знаем, какие действия обрабатывает эффект, но не можем при этом обрабатывать любое(случайное действие)
    //Из-за этого придётся с помощью триггера на каст способности к данной цели перехватывать применение способности и менять его.
    public interface IIncomingActionProcessingEffect<TAction> : IEffect where TAction : IBattleAction
    {
        public TAction ProcessIncomingAction(TAction originalAction); //CanBeNull
    }

    public interface IOutcomingActionProcessingEffect<TAction> : IEffect where TAction : IBattleAction
    {
        public TAction ProcessOutcomingAction(TAction originalAction); //CanBeNull
    }

    public interface ITickActionEffect : IEffect
    {
        public int TickLength { get; }
        public IBattleAction[] ActionsPerTick { get; }
    }

    public class DamageReduceEffectTest
    {
        public List<ITickEffect> Effects;

        public IIncomingActionProcessingEffect<TAction>[] GetProcessingEffects<TAction>()
            where TAction : IBattleAction
        {
            return Effects
                .Select(e => e as IIncomingActionProcessingEffect<TAction>)
                .Where(e => e != null)
                .ToArray();
        }
    }
}
