using Cysharp.Threading.Tasks;
using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static TMPro.Examples.ObjectSpin;

namespace OrderElimination.AbilitySystem
{
    public interface IUndoableBattleAction : IBattleAction
    {
        //static in implementation
        public bool Undo(int performId);

        public void ClearUndoCache();

        public static void ClearAllActionsUndoCache() => UndoableActionHelpers.ClearAllActionsUndoCache();

        private static class UndoableActionHelpers
        {
            private static readonly IUndoableBattleAction[] _undoableActionsSamples;
            private static readonly Type[] _implementedActionTypes;
            static UndoableActionHelpers()
            {
                _implementedActionTypes = ReflectionExtensions
                    .GetAllInterfaceImplementationTypes<IUndoableBattleAction>();

                //Say hello to C#11+ abstract static interface members
                _undoableActionsSamples = _implementedActionTypes
                    .Select(t => (IUndoableBattleAction)Activator.CreateInstance(t))
                    .ToArray();
            }
            public static void ClearAllActionsUndoCache()
            {
                foreach(var action in _undoableActionsSamples)
                {
                    action.ClearUndoCache();
                }
            }
        }
    }
}
