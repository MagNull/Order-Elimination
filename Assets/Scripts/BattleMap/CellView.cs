using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class CellView : MonoBehaviour
{
    public event Action<CellView> CellClicked;

    [SerializeField]
    private SpriteRenderer _renderer;
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
    private Color _deselectColor;
    private Color _basicCellTint;

    private Cell _model;

    public Color CurrentColor => _renderer == null ? Color.white : _renderer.color;
    public IReadOnlyCell Model => _model;

    public void Start()
    {
        _basicCellTint = _renderer == null ? Color.white : _renderer.color;
    }

    public void BindModel(Cell model)
    {
        _model ??= model;
    }

    public void Highlight(Color color)//pass enum CellHightlightColor instead
    {
        if (_renderer != null)
            _renderer.color = color;
    }

    public void Delight()
    {
        if (_renderer != null)
            _renderer.color = _basicCellTint;
    }

    public void Select()
    {
        _deselectColor = _renderer.color;
        var d =
            _renderer.color = _selectedColor;
    }

    public void Deselect()
    {
        _renderer.color = _deselectColor;
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        CellClicked?.Invoke(this);
    }
}