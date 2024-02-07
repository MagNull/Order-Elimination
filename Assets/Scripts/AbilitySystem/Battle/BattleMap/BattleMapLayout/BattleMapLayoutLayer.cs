using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;

namespace OrderElimination.Battle
{
    public class BattleMapLayoutLayer<T>
    {
        [ShowInInspector, OdinSerialize]
        public string LayerName { get; private set; }

        [ShowInInspector, OdinSerialize]
        public BattleSide Side { get; private set; }

        [TableMatrix(SquareCells = true, HideRowIndices = true, ResizableColumns = false)]
        [ShowInInspector, OdinSerialize]
        public T[,] Items { get; private set; }

        public BattleMapLayoutLayer(int width, int height)
        {
            Items = new T[width, height];
        }

        public void Resize(int width, int height)
        {
            if (width < 1 || height < 1)
                throw new ArgumentOutOfRangeException();
            var resizedLayer = new T[width, height];
            resizedLayer.FillIntersectionWith(Items);
            Items = resizedLayer;
        }

        public bool CanResizeWithoutLoss(int width, int height)
        {
            if (width < 1 || height < 1)
                throw new ArgumentOutOfRangeException();

            var currentWidth = Items.GetLength(0);
            var currentHeight = Items.GetLength(1);

            var minWidth = Math.Min(currentWidth, width);
            var minHeight = Math.Min(currentHeight, height);

            if (currentWidth <= width && currentHeight <= height)
                return true;

            if (width < currentWidth)
            {
                for (var x = width; x < currentWidth; x++)
                {
                    for (var y = 0; y < minHeight ; y++)
                    {
                        if (Items[x, y] != null)
                            return false;
                    }
                }
            }
            if (height < currentHeight)
            {
                for (var y = height; y < currentHeight; y++)
                {
                    for (var x = 0; x < minWidth; x++)
                    {
                        if (Items[x, y] != null)
                            return false;
                    }
                }
            }
            if (width < currentWidth && height < currentHeight)
            {
                for (var x = width; x < currentWidth; x++)
                {
                    for (var y = height; y < currentHeight; y++)
                    {
                        if (Items[x, y] != null)
                            return false;
                    }
                }
            }

            return true;
        }
    }
}
