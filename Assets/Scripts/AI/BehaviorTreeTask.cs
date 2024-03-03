using System;
using System.Collections.Generic;
using System.Linq;
using AI.Compositions;
using Cysharp.Threading.Tasks;
using OrderElimination;
using OrderElimination.AbilitySystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using XNode;

namespace AI
{
    [Serializable]
    public class TaskPort
    {
    }

    [NodeWidth(300)]
    public abstract class BehaviorTreeTask : Node
    {
        [Input(connectionType = ConnectionType.Override)]
        [SerializeField]
        private TaskPort Connector;

        public async UniTask<bool> TryRun(Blackboard blackboard)
        {
            var caster = blackboard.Get<AbilitySystemActor>("caster");
            if (!caster.IsAlive || caster.IsDisposedFromBattle)
                return false;
            if (caster.BattleSide == OrderElimination.Infrastructure.BattleSide.Player)
                Logging.LogError("AI tries to control Player characters!");
            return await Run(blackboard);
        }

        public abstract BehaviorTreeTask[] GetChildrenTasks();

        public override object GetValue(NodePort port)
        {
            return this;
        }

        protected abstract UniTask<bool> Run(Blackboard blackboard);
    }

    public abstract class SequentialTask : BehaviorTreeTask
    {
        [Output(connectionType = ConnectionType.Override)]
        [SerializeField]
        public TaskPort Next;

        public override BehaviorTreeTask[] GetChildrenTasks()
        {
            var ports = GetOutputPort("Next").GetConnections();
            var tasks = new List<BehaviorTreeTask>();
            foreach (var nodePort in ports)
            {
                var task = (BehaviorTreeTask)nodePort.node.GetValue(nodePort);
                tasks.Add(task);
            }

            return tasks.ToArray();
        }
    }

    public abstract class CompositionTask : BehaviorTreeTask
    {
        [Output(connectionType = ConnectionType.Override, dynamicPortList = true,
            backingValue = ShowBackingValue.Never)]
        public List<TaskPort> Branches = new();

        public override BehaviorTreeTask[] GetChildrenTasks()
        {
            var tasks = new List<BehaviorTreeTask>();
            for (var i = 0; i < Branches.Count; i++)
            {
                var nodePort = GetPort("Branches " + i);
                if (nodePort == null)
                {
                    Logging.LogException(
                        new NullReferenceException("Troubles with Branches in Composition Task " + GetType().Name));
                }

                var task = (BehaviorTreeTask)nodePort.Connection.node;
                tasks.Add(task);

            }

            return tasks.ToArray();
        }

        protected override void Init()
        {
            if (Branches.Count > 0)
                return;
            Branches.Add(new TaskPort());
        }
    }
}