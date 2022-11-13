using UnityEngine;

namespace OrderElimination
{
    [CreateAssetMenu(fileName = "SquadInfo", menuName = "Squad/New SquadInfo")]
    public class SquadInfo : ScriptableObject
    {
        [SerializeField] private Vector3 _position;
        [SerializeField] private Vector3 _buttonOnOrderPosition;
        public Vector3 Position => _position;
        public Vector3 PositionOnOrderPanel => _buttonOnOrderPosition;
    }
}