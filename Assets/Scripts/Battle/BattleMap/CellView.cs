using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class CellView : MonoBehaviour
{
    public event Action<CellView> CellClicked;

    [SerializeField]
    private SpriteRenderer _renderer;
    #region Old
    [SerializeField]
    private Color _selectedColor;
    [SerializeField]
    private Color _lightColor;
    [SerializeField]
    private Color _enemyColor;
    [FormerlySerializedAs("_environmentColor")]
    [SerializeField]
    private Color _obstacleColor;
    [SerializeField]
    private Color _characterSelectedColor;
    [SerializeField]
    private Color _allyColor;
    #endregion
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