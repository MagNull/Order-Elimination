﻿using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using UnityEngine;

namespace OrderElimination.SavesManagement
{
    [CreateAssetMenu(fileName = "new PlayerProgressPreset", menuName = "OrderElimination/Game/ProgressPreset")]
    public class PlayerProgressPreset : SerializedScriptableObject, IPlayerProgress
    {
        [OdinSerialize, ShowInInspector]
        public PlayerMetaProgress MetaProgress { get; set; }
        public PlayerRunProgress CurrentRunProgress { get; set; } = null;
        public DateTime SaveDate { get; set; } = DateTime.Now;

        private void Reset()
        {
            MetaProgress = PlayerProgressExtensions.GetDefaultMetaProgress();
        }
    }
}
