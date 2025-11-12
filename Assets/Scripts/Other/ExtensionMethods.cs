using UnityEngine;

namespace Other
{
    public static class ExtensionMethods
    {
        public static float Clamp(this Vector2 range, float value)
        {
            return Mathf.Clamp(value, range.x, range.y);
        }
    }
}