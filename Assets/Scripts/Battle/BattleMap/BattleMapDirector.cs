using System;
using OrderElimination;
using UnityEngine;
using VContainer;

[Serializable]
public class BattleMapDirector
{
    [SerializeField]
    private CellGridGenerator _generator;
    [SerializeField]
    private BattleMapView _battleMapView;
    private ScenesMediator _scenesMediator;
    public BattleMap Map => _battleMapView.Map;
    public BattleMapView MapView => _battleMapView;

    public void InitializeMap()
    {
        var grid = _generator.GenerateGrid(_battleMapView.Map.Width, _battleMapView.Map.Height);

        _battleMapView.Map.Init(grid.Model);
        _battleMapView.Init(grid.View);
    }

    [Inject]
    private void Construct(ScenesMediator scenesMediator)
    {
        _scenesMediator = scenesMediator;
    }
}