using System.Collections;
using System.Collections.Generic;
using AI.EditorGraph;
using Sirenix.OdinInspector;
using UnityEngine;

public class GraphTest : MonoBehaviour
{
    [SerializeField]
    private AIBehaviorTree _aiBehavior;

    [Button]
    public async void Test()
    {
        await _aiBehavior.Run();
    }
}
