using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;

public class TargetingSystemDisplay : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private AnimatedCrosshair _crosshairPrefab;
    [SerializeField]
    private AnimatedTrajectory _trajectoryPrefab;
    private BattleMapView _mapView;
    private IBattleMap _battleMap;

    private ObjectPool<AnimatedCrosshair> _crosshairsPool;
    private ObjectPool<AnimatedTrajectory> _trajectoriesPool;
    private readonly Dictionary<Vector2Int, AnimatedCrosshair> _placedCrosshairs = new();
    private readonly Dictionary<Vector2IntSegment, AnimatedTrajectory> _placedTrajectories = new();

    [Inject]
    private void Construct(BattleMapView mapView, IBattleMap battleMap)
    {
        _mapView = mapView;
        _battleMap = battleMap;
    }

    private void Awake()
    {
        _crosshairsPool = new(
            () => Instantiate(_crosshairPrefab, transform),
            ch => ch.gameObject.SetActive(true),
            ch => ch.gameObject.SetActive(false),
            ch => Destroy(ch.gameObject));
        _trajectoriesPool = new(
            () => Instantiate(_trajectoryPrefab, transform),
            t => t.gameObject.SetActive(true),
            t => t.gameObject.SetActive(false),
            t => Destroy(t.gameObject));
    }

    public void ShowCrosshairs(Vector2Int[] positions)
    {
        var crosshairsToHide = _placedCrosshairs.Keys.Except(positions).ToArray();
        var crosshairsToShow = positions.Except(_placedCrosshairs.Keys).ToArray();
        foreach (var pos in crosshairsToHide)
            HideCrosshair(pos);
        foreach (var pos in crosshairsToShow)
            ShowCrosshair(pos);
    }

    public void HideCrosshairs()
    {
        var placedCrosshairs = _placedCrosshairs.Values.ToArray();
        _placedCrosshairs.Clear();
        placedCrosshairs.ForEach(ch => _crosshairsPool.Release(ch));
    }

    public void ShowTrajectories(Vector2IntSegment[] segments)
    {
        var trajectoriesToHide = _placedTrajectories.Keys.Except(segments).ToArray();
        var trajectoriesToShow = segments.Except(_placedTrajectories.Keys).ToArray();
        foreach (var pos in trajectoriesToHide)
            HideTrajectory(pos);
        foreach (var pos in trajectoriesToShow)
            ShowTrajectory(pos);
    }

    public void HideTrajectories()
    {
        var placedTrajectories = _placedTrajectories.Values.ToArray();
        _placedTrajectories.Clear();
        placedTrajectories.ForEach(ch => _trajectoriesPool.Release(ch));
    }

    public void HideAllVisuals()
    {
        HideCrosshairs();
        HideTrajectories();
    }

    private bool ShowCrosshair(Vector2Int position)
    {
        if (!_battleMap.ContainsPosition(position)) throw new System.ArgumentOutOfRangeException();
        if (_placedCrosshairs.ContainsKey(position)) return false;
        var crosshair = _crosshairsPool.Get();
        crosshair.transform.position = _mapView.GameToWorldPosition(position);
        _placedCrosshairs.Add(position, crosshair);
        crosshair.PlayCrosshairInAnimation();
        return true;
    }

    private bool HideCrosshair(Vector2Int position)
    {
        if (!_battleMap.ContainsPosition(position)) throw new System.ArgumentOutOfRangeException();
        if (!_placedCrosshairs.ContainsKey(position)) return false;
        var crosshair = _placedCrosshairs[position];
        _placedCrosshairs.Remove(position);
        _crosshairsPool.Release(crosshair);
        return true;
    }

    private bool ShowTrajectory(Vector2IntSegment segment)
    {
        if (!_battleMap.ContainsPosition(segment.Start)
            || !_battleMap.ContainsPosition(segment.End)) 
            throw new System.ArgumentOutOfRangeException();
        if (_placedTrajectories.ContainsKey(segment)) 
            return false;
        var start = _mapView.GameToWorldPosition(segment.Start);
        var end = _mapView.GameToWorldPosition(segment.End);
        var trajectory = _trajectoriesPool.Get();
        trajectory.transform.position = start;
        _placedTrajectories.Add(segment, trajectory);
        trajectory.SetPoints(start, end);
        return true;
    }

    private bool HideTrajectory(Vector2IntSegment segment)
    {
        if (!_battleMap.ContainsPosition(segment.Start)
            || !_battleMap.ContainsPosition(segment.End))
            throw new System.ArgumentOutOfRangeException();
        if (!_placedTrajectories.ContainsKey(segment)) 
            return false;
        var trajectory = _placedTrajectories[segment];
        _placedTrajectories.Remove(segment);
        _trajectoriesPool.Release(trajectory);
        return true;
    }
}
