using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.Drawers;
using Sirenix.Utilities.Editor;
using System.Collections;
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
            var texture = value != null 
                ? TemplateAssetsDrawing.GetCroppedTexture(value.BattleIcon) 
                : null;
            var obj = SirenixEditorFields.UnityPreviewObjectField(
                rect, value, texture, typeof(StructureTemplate));
            return (StructureTemplate)obj;
        }
    }
}
