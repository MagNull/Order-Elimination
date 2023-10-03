using System;
using UnityEngine;

[Serializable]
public class BattleMapDirector
{
    [SerializeField]
    private CellGridGenerator _generator;
    [SerializeField]
    private BattleMapView _battleMapView;

    public void InitializeMap(int width, int height)
    {
        var mapModel = _battleMapView.Map;
        var grid = _generator.GenerateGrid(width, height);
        mapModel.Init(grid.Model);
        _battleMapView.Init(grid.View);
    }
}