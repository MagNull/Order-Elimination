using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem.FlowSystem
{
    public class AbilityLogicFlow
    {
        public List<IExecutionTrigger> Triggers { get; }
        public List<IBattleAction> Actions { get; }

        public void ActivateFlow()
        {
            foreach (var trigger in Triggers)
            {
                if (trigger is TargetHitTrigger hitTrigger)
                {
                    hitTrigger.Triggered += OnTriggerActivated;
                }
            }
        }

        private void OnTriggerActivated(IExecutionTrigger trigger)
        {
            ExecuteActions();
        }

        private void ExecuteActions()
        {

        }

        public class ActionNode
        {
            public IBattleAction action;
            public List<ActionFlow> flowsOnSuccess;

            public void Perform()
            {
                //var performResult = action.Perform(...);
                //if (performResult)
                //{
                //    foreach (var flow in flowsOnSuccess)
                //    {
                //        flow.Execute();
                //    }
                //}
            }
        }

        public class ActionFlow
        {
            public IReadOnlyList<IBattleAction> Actions => actionNodes.Select(n => n.action).ToList();
            public List<ActionNode> actionNodes;
            private Dictionary<IBattleAction, ActionNode> actionNodeMap;
            public IEnumerable<TAction> GetActions<TAction>() where TAction : IBattleAction
            {
                foreach (var action in Actions)
                {
                    if (action is TAction tAction)
                        yield return tAction;
                }
            }
            public void Execute()
            {
                foreach (var actionNode in actionNodes)
                    actionNode.Perform();
            }
            public void AddAction(IBattleAction action) => throw new NotImplementedException();
            public void RemoveAction(IBattleAction action) => throw new NotImplementedException();
            public void ReplaceAction(IBattleAction action) => throw new NotImplementedException();
        }
    }
}
