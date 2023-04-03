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
    private Color _basicColor;

    private Cell _model;

    public IReadOnlyCell Model => _model;

    public void Start()
    {
        _basicColor = _renderer == null ? Color.white : _renderer.color;
    }

    public void BindModel(Cell model)
    {
        _model ??= model;
    }

    public void Light()
    {
        if (_model.Objects.Any(obj => obj is BattleCharacter battleCharacter &&
                                      ((BattleCharacterView) battleCharacter.View).IsSelected))
            _renderer.color = _characterSelectedColor;
        else if (_model.Objects.Count == 1)
            _renderer.color = _lightColor;
        else if (_model.Objects.Count > 1 && _model.Objects.Any(obj => obj.Type == BattleObjectType.Enemy))
            _renderer.color = _enemyColor;
        else if (_model.Objects.Count > 1 && _model.Objects.Any(obj => obj.Type == BattleObjectType.Ally))
            _renderer.color = _allyColor;
        else
            _renderer.color = _lightColor;
    }

    public void Delight()
    {
        if (_renderer != null)
            _renderer.color = _basicColor;
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