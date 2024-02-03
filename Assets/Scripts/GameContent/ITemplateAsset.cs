using UnityEngine;

namespace OrderElimination.GameContent
{
    public interface ITemplateAsset : IGuidAsset
    {
        public string AssetName { get; }

        public Sprite AssetIcon { get; }
    }
}
