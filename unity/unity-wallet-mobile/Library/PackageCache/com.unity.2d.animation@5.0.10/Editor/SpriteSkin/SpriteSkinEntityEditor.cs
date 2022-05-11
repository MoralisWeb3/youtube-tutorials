using UnityEngine;

namespace UnityEditor.U2D.Animation
{

#if ENABLE_ENTITIES

    [CustomEditor(typeof(SpriteSkinEntity))]
    [CanEditMultipleObjects]
    class SpriteSkinEntityEditor : Editor
    {
        public override void OnInspectorGUI()
        {
          
        }
    }
#endif

}
