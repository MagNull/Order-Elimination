using System.Collections.Generic;
using RoguelikeMap.Points;

namespace RoguelikeMap.Map
{
    public interface IMapGenerator
    {
        public IEnumerable<Point> GenerateMap();
    }
}
