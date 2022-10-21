using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class test : MonoBehaviour
{
    [ShowInInspector]
    private List<test> list = new List<test>();
    [Button]
    private void OnEnable()
    {
        //print(name + " Enabled");
        var comps = GetComponentsInChildren<test>();
        print(transform.GetChild(0).GetChild(0).gameObject.activeSelf);
        DestroyImmediate(null);
    }
}
