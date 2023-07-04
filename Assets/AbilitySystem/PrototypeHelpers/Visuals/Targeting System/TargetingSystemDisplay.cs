using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingSystemDisplay : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private AnimatedCrosshair _crosshairPrefab;
    private BattleMapView _mapView;

    private void Construct(BattleMapView mapView)
    {
        _mapView = mapView;
    }

    public void ShowCrosshair(Vector2Int position)
    {
        var crosshair = Instantiate(_crosshairPrefab, transform);
        crosshair.transform.position = _mapView.GameToWorldPosition(position);
    }
}
