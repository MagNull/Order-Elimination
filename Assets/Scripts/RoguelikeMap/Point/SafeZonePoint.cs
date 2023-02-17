using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination
{
    public class SafeZonePoint : MonoBehaviour, IPoint
    {
        private PointInfo _pointInfo;
        private PointView _pointView;
        private List<IPoint> _nextPoints;
        private int _pointNumber;

        PointInfo IPoint.PointInfo
        {
            get => _pointInfo;
            set => _pointInfo = value;
        }

        PointView IPoint.PointView
        {
            get => _pointView;
            set => _pointView = value;
        }

        List<IPoint> IPoint.NextPoints
        {
            get => _nextPoints;
            set => _nextPoints = value;
        }

        int IPoint.PointNumber
        {
            get => _pointNumber;
            set => _pointNumber = value;
        }

        public void Visit(Squad squad)
        {
            squad.VisitSafeZonePoint();
        }
    }
}