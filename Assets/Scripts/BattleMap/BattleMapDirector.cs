using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMapDirector : MonoBehaviour
{
    [SerializeField] private CellGridGenerator _generator;
    [SerializeField] private BattleMap _battleMap;
    [SerializeField] private BattleMapView _battleMapView;

    public BattleMap Map => _battleMap;
    public BattleMapView MapView => _battleMapView;

    public void InitializeMap()
    {
        CellGrid grid = _generator.GenerateGrid(_battleMap.Width, _battleMap.Height);

        _battleMap.Init(grid.Model);
        _battleMapView.Init(grid.View);
    }
}
