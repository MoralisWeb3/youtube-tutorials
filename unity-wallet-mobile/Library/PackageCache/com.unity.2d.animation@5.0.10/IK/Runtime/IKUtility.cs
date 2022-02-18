using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.U2D.IK
{
    /// <summary>
    /// General utilities for 2D IK.
    /// </summary>
    [MovedFrom("UnityEngine.Experimental.U2D.IK")]
    public class IKUtility
    {
        /// <summary>
        /// Check if a Unity Transform is a descendent of another Unity Transform.
        /// </summary>
        /// <param name="transform">Unity Transform to check.</param>
        /// <param name="ancestor">Unity Transform ancestor.</param>
        /// <returns>Returns true if the Unity Transform is a descendent. False otherwise.</returns>
        public static bool IsDescendentOf(Transform transform, Transform ancestor)
        {
            Debug.Assert(transform != null, "Transform is null");

            Transform currentParent = transform.parent;

            while (currentParent)
            {
                if (currentParent == ancestor)
                    return true;

                currentParent = currentParent.parent;
            }

            return false;
        }

        /// <summary>
        /// Gets the hierarchy depth of a Unity Transform.
        /// </summary>
        /// <param name="transform">Unity Transform to check.</param>
        /// <returns>Integer value for hierarchy depth.</returns>
        public static int GetAncestorCount(Transform transform)
        {
            Debug.Assert(transform != null, "Transform is null");

            int ancestorCount = 0;

            while (transform.parent)
            {
                ++ancestorCount;

                transform = transform.parent;
            }

            return ancestorCount;
        }

        /// <summary>
        /// Gets the maximum chain count for a IKChain2D.
        /// </summary>
        /// <param name="chain">IKChain2D to query.</param>
        /// <returns>Integer value for the maximum chain count.</returns>
        public static int GetMaxChainCount(IKChain2D chain)
        {
            int maxChainCount = 0;

            if (chain.effector)
                maxChainCount = GetAncestorCount(chain.effector) + 1;

            return maxChainCount;
        }
    }
}
