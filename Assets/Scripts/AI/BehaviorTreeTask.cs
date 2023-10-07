using System.Collections.Generic;
using AI.Compositions;
using Cysharp.Threading.Tasks;
using OrderElimination;
using OrderElimination.AbilitySystem;
using UnityEngine;
using XNode;

namespace AI
{
    [NodeWidth(300)]
    public abstract class BehaviorTreeTask : Node
    {
        [Input]
        [SerializeField]
        private TaskPort InputPort;

        public async UniTask<bool> TryRun(Blackboard blackboard)
        {
            var caster = blackboard.Get<AbilitySystemActor>("caster");
            if (!caster.IsAlive || caster.IsDisposedFromBattle)
                return false;
            if (caster.BattleSide == OrderElimination.Infrastructure.BattleSide.Player)
                Logging.LogError("AI tries to control Player characters!");
            return await Run(blackboard);
        }

        protected abstract UniTask<bool> Run(Blackboard blackboard);

        protected BehaviorTreeTask[] GetChildrenTasks()
        {
            var ports = GetOutputPort("ChildrenPort").GetConnections();
            var tasks = new List<BehaviorTreeTask>();
            foreach (var nodePort in ports)
            {
                var task = (BehaviorTreeTask)nodePort.node.GetValue(nodePort);
                tasks.Add(task);
            }

            return tasks.ToArray();
        }

        public override object GetValue(NodePort port)
        {
            return this;
        }
    }
}