using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination
{
    public interface ISelectable
    {
        public void Select();

        public void Unselect();
    }

}