using System.Collections.Generic;
using System.Linq;
using RoguelikeMap.Points.Models;
using UnityEngine;
using XNode;

namespace RoguelikeMap.Points
{
	[CreateAssetMenu]
	public class PointGraph : NodeGraph
	{
		public StartPointModel StartPoint { get; private set; }

		public IEnumerable<PointModel> GetPoints()
		{
			return nodes.Cast<PointModel>();
		}
	}
}