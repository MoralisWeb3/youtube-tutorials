#if ENABLE_ANIMATION_COLLECTION && ENABLE_ANIMATION_BURST
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace UnityEditor.U2D.Animation
{
    internal class SpriteSkinCompositeDebugWindow : EditorWindow
    {
        [MenuItem("internal:Window/2D/SpritSkinCompositeDebug")]
        static void Launch()
        {
            EditorWindow.GetWindow<SpriteSkinCompositeDebugWindow>().Show();
        }

        Vector2 m_ScrollPos = Vector2.zero;
        string m_DebugLog = "";

        void OnGUI()
        {
            if (GUILayout.Button("Reset Sprite SkinComposite"))
                SpriteSkinComposite.instance.ResetComposite();

            if (GUILayout.Button("Show Debug"))
            {
                m_DebugLog = SpriteSkinComposite.instance.GetDebugLog();
            }

            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
            GUILayout.TextArea(m_DebugLog);
            EditorGUILayout.EndScrollView();
        }
   
    }
}
#endif
