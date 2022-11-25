using System;
using System.Linq;
using OrderElimination.BattleMap;
using UnityEngine;
using UnityEngine.Rendering;
using VContainer;

[Serializable]
public class BattleMapDirector
{
    [SerializeField]
    private CellGridGenerator _generator;
    [SerializeField]
    private BattleMapView _battleMapView;
    [SerializeField]
    private SerializedDictionary<Vector2Int, EnvironmentInfo> _environmentObjects = new();

    private EnvironmentFactory _environmentFactory;
    public BattleMap Map => _battleMapView.Map;
    public BattleMapView MapView => _battleMapView;

    public void InitializeMap()
    {
        var grid = _generator.GenerateGrid(_battleMapView.Map.Width, _battleMapView.Map.Height);


        _battleMapView.Map.Init(grid.Model);
        _battleMapView.Init(grid.View);
        
        AddEnvironmentObjects();
    }

    [Inject]
    private void Construct(EnvironmentFactory environmentFactory)
    {
        _environmentFactory = environmentFactory;
    }

    private void AddEnvironmentObjects()
    {
        if (_environmentObjects.Count == 0)
            return;

        var environmentObjects = _environmentObjects
            .Select(v => (CoordinateSpace: v.Key, Object: _environmentFactory.Create(v.Value)));
        foreach (var environmentObject in environmentObjects)
        {
            var coords = environmentObject.CoordinateSpace;
            _battleMapView.Map.MoveTo(environmentObject.Object, coords.x, coords.y);
        }
    }
}