using System.Collections.Generic;
using AI.Compositions;
using Cysharp.Threading.Tasks;
using UnityEngine;
using XNode;

namespace AI
{
    public abstract class BehaviorTreeTask : Node
    {
        [Input]
        [SerializeField]
        private TaskPort InputPort;
        public abstract UniTask<bool> Run(Blackboard blackboard);
        
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
    }
}