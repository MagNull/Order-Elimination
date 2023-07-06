using UnityEngine;

namespace OrderElimination.Infrastructure
{
    public struct Vector2IntSegment
    {
        public Vector2IntSegment(Vector2Int start, Vector2Int end)
        {
            Start = start;
            End = end;
        }

        public Vector2IntSegment(int xStart, int yStart, int xEnd, int yEnd)
        {
            Start = new Vector2Int(xStart, yStart);
            End = new Vector2Int(xEnd, yEnd); ;
        }

        public Vector2Int Start { get; set; }
        public Vector2Int End { get; set; }
        public float Length => (End - Start).magnitude;

        public override int GetHashCode()
        {
            //bad?
            return unchecked(Start.GetHashCode() * 31 + End.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector2IntSegment segment)
            {
                return this == segment;
            }
            return false;
        }

        public override string ToString()
            => $"[{Start}; {End}]";

        public static bool operator ==(Vector2IntSegment segA, Vector2IntSegment segB)
            => segA.Start == segB.Start && segA.End == segB.End;

        public static bool operator !=(Vector2IntSegment segA, Vector2IntSegment segB)
            => !(segA == segB);
    }
}
