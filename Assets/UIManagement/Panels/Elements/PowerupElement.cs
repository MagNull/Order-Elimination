using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UIManagement.trashToRemove_Mockups;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement.Elements
{
    [ExecuteInEditMode]
    public class PowerupElement: MonoBehaviour
    {
        [SerializeField] private Image _powerupIconComponent;
        public Image ImageComponent => _powerupIconComponent;
        public Powerup PowerupInfo { get; private set; }

        [ShowInInspector, PreviewField(60)]
        public Sprite Icon
        {
            get => _powerupIconComponent == null ? null : _powerupIconComponent.sprite;
            set => _powerupIconComponent.sprite = value;
        }

        public event Action<PowerupElement> Destroyed;

        public void UpdatePowerupInfo(Powerup powerupInfo)
        {
            if (powerupInfo == null)
                throw new InvalidOperationException();
            Icon = powerupInfo.Icon;
        }

        private void OnDestroy()
        {
            Destroyed?.Invoke(this);
        }
    }
}
