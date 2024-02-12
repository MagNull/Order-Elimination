using System;
using UnityEngine;

namespace OrderElimination
{
    //#TOREMOVE
    public static class StringExtenstion
    {
        public static Vector3 GetVectorFromString(this string str)
        {
            var temp = str.Split(new[] { "(", ")", ", ", ".00" }, StringSplitOptions.RemoveEmptyEntries);

            var x = Convert.ToInt32(temp[0]);
            var y = Convert.ToInt32(temp[1]);
            var z = Convert.ToInt32(temp[2]);
            return new Vector3(x, y, z);
        }
    }
}