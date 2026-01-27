using UnityEngine;

namespace Other
{
    public static class ExtensionMethods
    {
        public static float Clamp(this Vector2 range, float value)
        {
            return Mathf.Clamp(value, range.x, range.y);
        }

        public static float Distance(this Color color, Color otherColor)
        {
            var num1 = Mathf.Abs(color.r - otherColor.r);
            var num2 = Mathf.Abs(color.g - otherColor.g);
            var num3 = Mathf.Abs(color.b - otherColor.b);
            return num1 + num2 + num3;
        }
    }
}