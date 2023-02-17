using System.Collections.Generic;
using System.Linq;
using OrderElimination;
using UnityEngine;
using VContainer;

namespace RoguelikeMap
{
    public class Map : MonoBehaviour
    {
        public string SquadPositionPrefPath = $"{SaveIndex}/Squad/Position";
        private List<OrderElimination.Point> _points;
        private IMapGenerator _mapGenerator;
        private Squad _squad;
        public static int SaveIndex { get; private set; }
        
        [Inject]
        private void Construct(IMapGenerator mapGenerator, Squad squad)
        {
            _mapGenerator = mapGenerator;
            _squad = squad;
        }

        private void Start()
        {
            _points = _mapGenerator.GenerateMap();
            SetSquadPosition();
        }

        private void SetSquadPosition()
        {
            Vector3 position;
            position = PlayerPrefs.HasKey(SquadPositionPrefPath)
                ? PlayerPrefs.GetString(SquadPositionPrefPath).GetVectorFromString()
                : _points.First().transform.position;
            _squad.Move(position);
        }
    }
}