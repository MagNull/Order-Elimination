using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CellView : MonoBehaviour
{
    public event Action<CellView> CellClicked;

    [SerializeField]
    private SpriteRenderer _renderer; 
    [SerializeField]
    private SpriteRenderer _backgroundRenderer;
    [SerializeField]
    private BoxCollider2D _collider;

    private Color _basicCellTint;
    private Cell _model;

    public Color CurrentColor => _renderer == null ? Color.white : _renderer.material.color;
    [ShowInInspector]
    public Vector2 Size => _collider.size * transform.localScale;
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
        if (_backgroundRenderer != null)
            _backgroundRenderer.material.color = color;
    }

    public void Delight()
    {
        if (_renderer != null)
            _renderer.material.color = _basicCellTint;
        if (_backgroundRenderer != null)
            _backgroundRenderer.material.color = _basicCellTint;
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        CellClicked?.Invoke(this);
    }
}