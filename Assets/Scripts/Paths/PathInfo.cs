using OrderElimination;
using UnityEngine;

[CreateAssetMenu(fileName = "PathInfo", menuName = "Path/New PathInfo")]
public class PathInfo : ScriptableObject
{
    [SerializeField] private PlanetInfo _end;
    public PlanetInfo End => _end;
}
