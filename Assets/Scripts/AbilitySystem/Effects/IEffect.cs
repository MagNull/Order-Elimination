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
        //public EffectCharacter EffectCharacter { get; }
        public IBattleAction[] ActionsOnApply { get; }
        public IBattleAction[] ActionsOnRemove { get; }
        //RemovedByTriggers
        public event Action<IEffect> Removed;//Destroyed Disposed Finished
    }

    public interface ITemporaryEffect : IEffect
    {
        public int Duration { get; }
        public event Action<ITemporaryEffect> EffectEnded;
    }

    public interface IIncomingActionProcessingEffect<TAction> : IEffect where TAction : IBattleAction
    {
        public TAction ProcessIncomingAction(TAction originalAction);
    }

    public interface IOutcomingActionProcessingEffect<TAction> : IEffect where TAction : IBattleAction
    {
        public TAction ProcessOutcomingAction(TAction originalAction);
    }

    public interface ITickActionEffect : IEffect
    {
        public int TickLength { get; } // 1Tick = 1 ход
        //Действия обязаны выполняться каждый промежуток ходов, пока активен эффект.
        //Действия должны прекратить выполняться при удалении эффекта.
        public IBattleAction[] ActionsPerTick { get; }
    }

    //public class DamageReduceEffectTest
    //{
    //    public List<ITickEffect> Effects;

    //    public IIncomingActionProcessingEffect<TAction>[] GetProcessingEffects<TAction>()
    //        where TAction : IBattleAction
    //    {
    //        return Effects
    //            .Select(e => e as IIncomingActionProcessingEffect<TAction>)
    //            .Where(e => e != null)
    //            .ToArray();
    //    }
    //}
}
