using OrderElimination.Infrastructure;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStarter : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    public KeyCode StartTestKey = KeyCode.T;

    [Header("Test Setup")]
    [SerializeField]
    public Vector2Int CasterPos;

    [SerializeField]
    public Vector2Int TargetPos;

    void Update()
    {
        if (Input.GetKeyDown(StartTestKey))
        {
            Test();
        }
    }

    private void Test()
    {
        var pattern = new DistanceFromPointPattern(0.06f, 1, true);
        var positions = pattern.GetAbsolutePositions(CasterPos);
        var inPattern = pattern.ContainsPositionWithOrigin(TargetPos, CasterPos);
        Debug.Log($"Positions: {positions}\nTarget in Pattern: {inPattern}");
    }
}
