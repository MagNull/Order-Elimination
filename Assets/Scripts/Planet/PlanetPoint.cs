using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination
{
    //Подумать надо ли
    enum PointType
    {
        Base,
        Hearth,
        EnemyBase
    }

    public class PlanetPoint
    {
        private string _name;
        private Squad _opponents;
        private PointType _type;

        public int сhanceOfItems { get; private set; }
        public int chanceOfFightingBack { get; private set; }
        public float expirience { get; private set; }

        public PlanetPoint(string name, Squad opponents)
        {
            _name = name;
            _opponents = opponents;
        }

        public void AddOpponent(Character unit)
        {
            _opponents.AddUnit(unit);
        }

        public void RemoveOpponent(Character unit)
        {
            _opponents.RemoveUnit(unit);
        }
    }
}
