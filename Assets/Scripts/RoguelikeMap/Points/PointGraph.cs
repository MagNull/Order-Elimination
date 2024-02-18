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

		public void Initialize()
		{
			CurrentPoint = StartPoint;
		}

		public IEnumerable<PointModel> GetPoints()
		{
			List<PointModel> points = new();
			foreach (var node in nodes)
			{
				if (node is PointModel pointModel)
					points.Add(pointModel);
			}
			return points;
		}

		public PointModel GetNextPoint()
		{
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

			SetNextPoint(exitPort);

			return CurrentPoint;
		}

		private void SetNextPoint(NodePort exitPort)
		{
			if (exitPort.Connection.node is RandomNode randomNode)
			{
				CurrentPoint = randomNode.GetRandomNextPoint();
				return;
			}

			CurrentPoint = exitPort.Connection.node as PointModel;
		}
	}
}