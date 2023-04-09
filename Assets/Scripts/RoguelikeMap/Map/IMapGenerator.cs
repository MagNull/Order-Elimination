using System.Collections.Generic;
using RoguelikeMap.Points;

namespace OrderElimination
{
    public interface IMapGenerator
    {
        public List<Point> GenerateMap();
    }
}
