using DG.Tweening;
using UnityEngine.UI;
using VContainer;

namespace OrderElimination
{
    public class SquadCommander
    {
        private readonly IObjectResolver _objectResolver;
        private Image _attackImage;
        private PlanetPoint _target;
        private Squad _squad;
        public PlanetPoint Target => _target;
        public Squad Squad => _squad;

        [Inject]
        public SquadCommander(IObjectResolver objectResolver, Image attackImage)
        {
            _objectResolver = objectResolver;
            _attackImage = attackImage;
        }

        public void Set(Squad squad, PlanetPoint target)
        {
            _squad = squad;
            _target = target;
            PlayAnimation();
        }

        public void PlayAnimation()
        {
            var tween = _attackImage.DOFade(1, 0.5f);
        }
    }
}