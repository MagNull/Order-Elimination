using System;
using System.Collections.Generic;
using System.Linq;
using OrderElimination;
using OrderElimination.BM;
using UnityEngine;
using UnityEngine.Rendering;
using VContainer;
using Random = System.Random;

[Serializable]
public class BattleMapDirector
{
    [SerializeField]
    private CellGridGenerator _generator;
    [SerializeField]
    private BattleMapView _battleMapView;
    [SerializeField]
    private List<Dictionary<Vector2Int, EnvironmentInfo>> _environmentObjects = new();

    private CharactersMediator _charactersMediator;

    private EnvironmentFactory _environmentFactory;
    public BattleMap Map => _battleMapView.Map;
    public BattleMapView MapView => _battleMapView;

    public int InitializeMap()
    {
        var grid = _generator.GenerateGrid(_battleMapView.Map.Width, _battleMapView.Map.Height);

        _battleMapView.Map.Init(grid.Model);
        _battleMapView.Init(grid.View);
        
        AddEnvironmentObjects(_charactersMediator.PointNumber);
        return _charactersMediator.PointNumber;
    }

    [Inject]
    private void Construct(EnvironmentFactory environmentFactory, CharactersMediator charactersMediator)
    {
        _environmentFactory = environmentFactory;
        _charactersMediator = charactersMediator;
    }

    private void AddEnvironmentObjects(int mapIndex)
    {
        if (_environmentObjects.Count == 0)
            return;

        var environmentObjects = _environmentObjects[mapIndex]
            .Select(v => (CoordinateSpace: v.Key, Object: _environmentFactory.Create(v.Value)));
        foreach (var environmentObject in environmentObjects)
        {
            var coords = environmentObject.CoordinateSpace;
            _battleMapView.Map.MoveTo(environmentObject.Object, coords.x, coords.y);
        }
    }

    private int GetRandomMapIndex()
    {
        var rnd = new Random();
        return rnd.Next(0, _environmentObjects.Count);
    }
}