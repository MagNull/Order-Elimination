using UnityEngine;

namespace OrderElimination.Utils
{
    public static class ColorExtensions
    {
        public static Color GetContrastColor(this Color color)
        {
            Color.RGBToHSV(color, out var hue, out var sat, out var val);
            return Color.Lerp(Color.white, new Color(0.2f, 0.2f, 0.2f), val);
        }

        public static Color WithAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }
    }
}
