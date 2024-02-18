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
		[field: SerializeField]
		public StartPointModel StartPoint { get; private set; }

		public PointModel CurrentPoint { get; private set; }

		public IEnumerable<PointModel> GetPoints()
		{
			return nodes.Cast<PointModel>();
		}

		public PointModel GetNextPoint()
		{
			if (CurrentPoint == null)
			{
				CurrentPoint = StartPoint;
			}

			if (CurrentPoint is FinalBattlePointModel)
			{
				return null;
			}

			NodePort exitPort = CurrentPoint.GetOutputPort("exits");

			if (!exitPort.IsConnected && CurrentPoint is not FinalBattlePointModel)
			{
				Debug.LogException(new System.Exception("PointModel isn't connected"));
				return null;
			}

			if (exitPort.Connection.node is RandomNode randomNode)
			{
				return randomNode.GetRandomNextPoint();
			}

			return exitPort.Connection.node as PointModel;
		}
	}
}