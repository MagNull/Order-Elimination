using System;
using UnityEngine;

public class ClickProcess : MonoBehaviour
{
    public event Action OnClick;
    
    private void OnMouseDown()
    {
        Debug.Log("Click");
        OnClick?.Invoke();
    }
}
