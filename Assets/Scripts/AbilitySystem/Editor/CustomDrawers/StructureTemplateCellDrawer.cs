using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.Drawers;
using Sirenix.Utilities.Editor;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace OrderElimination.Editor
{
    internal sealed class StructureTemplateCellDrawer<TArray>
        : TwoDimensionalArrayDrawer<TArray, StructureTemplate>
        where TArray : IList
    {
        protected override TableMatrixAttribute GetDefaultTableMatrixAttributeSettings()
        {
            return new TableMatrixAttribute()
            {
                SquareCells = true,
                HideRowIndices = true,
                ResizableColumns = false,
            };
        }

        protected override StructureTemplate DrawElement(
            Rect rect, StructureTemplate value)
        {
            Texture2D texture = null;
            if (value != null)
            {
                texture = value.BattleIcon != null
                    ? TemplateAssetsDrawing.GetCroppedTexture(value.BattleIcon) 
                    : AssetPreview.GetMiniThumbnail(value);
            }
            var fieldObj = SirenixEditorFields.UnityPreviewObjectField(
                    rect, value, texture, typeof(StructureTemplate));
            return (StructureTemplate)fieldObj;
        }
    }
}
