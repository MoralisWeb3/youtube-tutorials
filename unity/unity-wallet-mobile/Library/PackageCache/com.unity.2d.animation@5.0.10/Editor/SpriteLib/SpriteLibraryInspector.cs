using UnityEngine.Experimental.U2D.Animation;

namespace UnityEditor.Experimental.U2D.Animation
{
    [CustomEditor(typeof(SpriteLibrary))]
    [CanEditMultipleObjects]
    internal class SpriteLibraryInspector : Editor
    {
        private SerializedProperty m_SpriteLib;

        public void OnEnable()
        {
            m_SpriteLib = serializedObject.FindProperty("m_SpriteLibraryAsset");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_SpriteLib);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();

                foreach (var t in targets)
                {
                    var srs = (t as SpriteLibrary).GetComponentsInChildren<SpriteResolver>();
                    foreach (var sr in srs)
                    {
                        sr.ResolveSpriteToSpriteRenderer();
                        sr.spriteLibChanged = true;
                    }
                }
            }
        }
    }
}
