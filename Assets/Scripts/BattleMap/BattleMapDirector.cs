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
    private CharactersMediator _charactersMediator;
    public BattleMap Map => _battleMapView.Map;
    public BattleMapView MapView => _battleMapView;

    public void InitializeMap()
    {
        var grid = _generator.GenerateGrid(_battleMapView.Map.Width, _battleMapView.Map.Height);

        _battleMapView.Map.Init(grid.Model);
        _battleMapView.Init(grid.View);
    }

    [Inject]
    private void Construct(CharactersMediator charactersMediator)
    {
        _charactersMediator = charactersMediator;
    }
}