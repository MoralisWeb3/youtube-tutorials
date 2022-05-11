using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal static class TransformExtensions
    {
        public static Vector3 GetScaledRight(this Transform transform)
        {
            return transform.localToWorldMatrix.MultiplyVector(Vector3.right);
        }

        public static Vector3 GetScaledUp(this Transform transform)
        {
            return transform.localToWorldMatrix.MultiplyVector(Vector3.up);
        }

        public static bool IsDescendentOf(this Transform transform, Transform ancestor)
        {
            if (ancestor != null)
            {
                var parent = transform.parent;

                while (parent != null)
                {
                    if (parent == ancestor)
                        return true;

                    parent = parent.parent;
                }
            }

            return false;
        }
    }
}
