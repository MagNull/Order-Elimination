using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class CellView : MonoBehaviour
{
    public event Action<CellView> CellClicked;

    [SerializeField]
    private Renderer _renderer;
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
    private Color _allyColor;
    private Color _deselectColor;
    private Color _basicColor;

    private Cell _model;

    public Cell Model => _model;

    public void Start()
    {
        _basicColor = _renderer == null ? Color.white : _renderer.material.color;
    }

    public void BindModel(Cell model)
    {
        _model ??= model;
    }

    public void Light()
    {
        _renderer.material.color = _model.GetObject().Side switch
        {
            BattleObjectSide.None => _lightColor,
            BattleObjectSide.Obstacle => _obstacleColor,
            BattleObjectSide.Environment => _lightColor,
            BattleObjectSide.Enemy => _enemyColor,
            BattleObjectSide.Ally => _allyColor,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public void Delight()
    {
        if(_renderer != null)
            _renderer.material.color = _basicColor;
    }

    public void Select()
    {
        _deselectColor = _renderer.material.color;
        _renderer.material.color = _selectedColor;
    }

    public void Deselect() => _renderer.material.color = _deselectColor;

    private void OnMouseDown()
    {
        if(EventSystem.current.IsPointerOverGameObject())
            return;
        Debug.Log(Model.GetObject() + " clicked");
        CellClicked?.Invoke(this);
    }
}