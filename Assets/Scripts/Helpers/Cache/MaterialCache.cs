using UnityEngine;

namespace Helpers.Cache
{
    public static class MaterialCache
    {
        private static Material outlineRed;
        private static Material outlineGreen;

        public static Material GetOutlineRed()
        {
            if (outlineRed == null) outlineRed = Resources.Load<Material>("Materials/OutlineRed");
            return outlineRed;
        }

        public static Material GetOutlineGreen()
        {
            if (outlineGreen == null) outlineGreen = Resources.Load<Material>("Materials/OutlineGreen");
            return outlineGreen;
        }
    }
}