﻿using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AI.Compositions
{
    [Serializable]
    public class TaskPort
    {
    }

    [Serializable]
    public class Selector : BehaviorTreeTask
    {
        [Output]
        [SerializeField]
        private TaskPort ChildrenPort;
        
        private BehaviorTreeTask[] _childrenTask;

        public override async UniTask<bool> Run(Blackboard blackboard)
        {
            _childrenTask = GetChildrenTasks();
            foreach (var task in _childrenTask)
            {
                var result = await task.Run(blackboard);
                if (result)
                    return true;
            }

            return false;
        }
    }
}