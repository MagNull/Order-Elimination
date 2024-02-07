using OrderElimination.GameContent;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace OrderElimination.Editor
{
    public static class TemplateAssetsDrawing
    {
        //TODO: Utilize after ITemplateAsset utilized
        public static T DrawTemplateAssetField<T>(Rect rect, T value)
            where T : Object, ITemplateAsset
        {
            var texture = value != null ? GetCroppedTexture(value.AssetIcon) : null;
            var obj = SirenixEditorFields.UnityPreviewObjectField(rect, value, texture, typeof(T));
            return (T)obj;
        }

        public static Texture2D GetCroppedTexture(Sprite sprite)
        {
            if (sprite == null) return null;
            var rect = sprite.rect;
            var texture = sprite.texture;
            if (rect.height == texture.height && rect.width == texture.width
                && rect.x == 0 && rect.y == 0)
                return texture;
            var yMax = rect.yMax;
            var yMin = rect.yMin;
            rect.yMax = texture.height - 1 - yMin;
            rect.yMin = texture.height - 1 - yMax;
            return texture.CropTexture(rect);
        }
    }
}
