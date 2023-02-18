using DG.Tweening;
using RoguelikeMap;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace OrderElimination
{
    public class SquadCommander
    {
        private readonly IObjectResolver _objectResolver;
        private GameObject _image;
        private Point _target;
        private Squad _squad;
        public Point Target => _target;
        public Squad Squad => _squad;

        [Inject]
        public SquadCommander(IObjectResolver objectResolver, GameObject image)
        {
            _objectResolver = objectResolver;
            _image = image;
        }

        public void Set(Squad squad, Point target)
        {
            _squad = squad;
            _target = target;
            PlayAnimation();
        }

        public void ShowBattleImage()
        {
            
        }

        public void ShowEventImage(DialogWindowFormat window)
        {
            var text = _image.GetComponent<TMP_Text>();
            text.text = window.Text;
        }

        public void ShowShopImage(DialogWindowFormat window)
        {
            var text = _image.GetComponent<TMP_Text>();
            text.text = window.Text;
        }

        public void ShowSafeZoneImage(string text)
        {
            
        }
        
        public void PlayAnimation()
        {
            // var tween = _attackImage.DOFade(1, 0.5f);
            // tween.OnComplete(StartAttack);
        }

        public void StartAttack()
        {
            // var order = CreateAttackOrder();
            // order.Start();
        }
    }
}