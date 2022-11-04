using UnityEngine;

[CreateAssetMenu(fileName = "PathInfo", menuName = "Path/New PathInfo")]
public class PathInfo : ScriptableObject
{
    [SerializeField] private Vector3 _position;
    public Vector3 Positon => _position;
}
