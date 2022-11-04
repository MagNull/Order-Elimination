using UnityEngine;

namespace OrderElimination
{
    [CreateAssetMenu(fileName = "SquadInfo", menuName = "Squad/New SquadInfo")]
    public class SquadInfo : ScriptableObject
    {
        [SerializeField] private Vector3 _position;
        public Vector3 Position => _position;
    }
}