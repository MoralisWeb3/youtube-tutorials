namespace UnityEngine.U2D.Animation
{
    internal class SpriteSkinManager
    {
        // Doing this to hide it from user adding it from Inspector
        [DefaultExecutionOrder(-1)]
        [ExecuteInEditMode]
        internal class SpriteSkinManagerInternal : MonoBehaviour
        {
#if ENABLE_ANIMATION_COLLECTION
            void LateUpdate()
            {
                if (SpriteSkinComposite.instance.helperGameObject != gameObject)
                {
                    GameObject.DestroyImmediate(gameObject);
                    return;
                }
                SpriteSkinComposite.instance.LateUpdate();
            }
#endif
        }
    }
}
