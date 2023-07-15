using System;

namespace OrderElimination.AbilitySystem
{
    //MoveAction - move per cell, InflictDamageAction - per damage infliction
    public interface IIterativeBattleAction : IBattleAction
    {
        public event Action<IIterativeBattleAction> IterationPerformed;
    }
}
