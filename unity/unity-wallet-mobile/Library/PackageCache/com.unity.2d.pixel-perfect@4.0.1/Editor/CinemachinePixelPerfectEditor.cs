namespace UnityEditor.U2D
{
    [CustomEditor(typeof(UnityEngine.U2D.CinemachinePixelPerfect)), CanEditMultipleObjects]
    internal class CinemachinePixelPerfectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("This Cinemachine extension is now deprecated and doesn't function properly. Instead, use the one from Cinemachine v2.4.0 or newer.", MessageType.Error);
        }
    }
}
