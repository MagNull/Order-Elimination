using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProcChanceWindow : MonoBehaviour
{
    [SerializeField] private GameObject _chanceWindow;

    private GameObject _currentChanceWindow;

    public void Show(float probability, GameObject parent)
    {
        Debug.Log(parent.transform.position);
        _currentChanceWindow = Instantiate(_chanceWindow, parent.transform);
        var text = _currentChanceWindow.GetComponentInChildren<TMP_Text>();
        text.rectTransform.anchorMax = parent.transform.position;
        text.rectTransform.anchoredPosition += new Vector2(0, parent.transform.localScale.y / 1.5f);
        var meshRenderer = _currentChanceWindow.GetComponentInChildren<MeshRenderer>();
        meshRenderer.sortingOrder = 10;
        Debug.Log(text.transform.position);
        text.text = probability.ToString();
    }

    public void Hide()
    {
        if (_currentChanceWindow != null)
        {
            Destroy(_currentChanceWindow);
        }
    }
}
