using System;

namespace OrderElimination.GameContent
{
    public interface IGuidAsset
    {
        public Guid AssetId { get; }

        public void UpdateId(Guid newId);
    }
}
