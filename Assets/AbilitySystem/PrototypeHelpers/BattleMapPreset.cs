//using OrderElimination.Infrastructure;
//using Sirenix.OdinInspector;
//using Sirenix.Utilities.Editor;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using UnityEngine;

//namespace OrderElimination.MacroGame
//{
//    public class BattleMapPreset : SerializedScriptableObject
//    {
//        public const int Height = 8;
//        public const int Width = 8;
//        public class CellData
//        {
//            public EnumMask<SpawnType> SpawnType;
//            public List<IBattleStructureTemplate> Structures;
//        }

//        [TableMatrix()]
//        public CellData[,] Cells = new CellData[Height, Width];

//        private static CellData DrawCell(Rect rect, CellData cellData)
//        {
//            foreach (var spawnType in EnumExtensions.GetValues<SpawnType>())
//            {
//                #region draw spawns text
//                var cellText = new StringBuilder();
//                //cellText.Append(spawnInfo.Position);
//                if (cellData.SpawnType[spawnType])
//                {
//                    cellText.Append($"\n{spawnType}");
//                }
//                DrawLabel
//                #endregion
//            }
//            return cellData;
//        }
//    }

//    public enum SpawnType
//    {
//        Player,
//        Enemies
//    }
//}
