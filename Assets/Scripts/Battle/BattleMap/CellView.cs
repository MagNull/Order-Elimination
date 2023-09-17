using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CellView : MonoBehaviour
{
    public event Action<CellView> CellClicked;

    [SerializeField]
    private SpriteRenderer _renderer;
    private Color _basicCellTint;

    private Cell _model;

    public Color CurrentColor => _renderer == null ? Color.white : _renderer.material.color;
    public IReadOnlyCell Model => _model;

    public void Start()
    {
        _basicCellTint = _renderer == null ? Color.white : _renderer.material.color;
    }

    public void BindModel(Cell model)
    {
        _model ??= model;
    }

    public void Highlight(Color color)//pass enum CellHightlightColor instead
    {
        if (_renderer != null)
            _renderer.material.color = color;
    }

    public void Delight()
    {
        if (_renderer != null)
            _renderer.material.color = _basicCellTint;
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        CellClicked?.Invoke(this);
    }
}