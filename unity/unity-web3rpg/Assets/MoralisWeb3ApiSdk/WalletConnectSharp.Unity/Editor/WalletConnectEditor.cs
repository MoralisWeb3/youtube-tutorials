

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using WalletConnectSharp.Unity;

[CustomEditor(typeof(WalletConnect))]
public class WalletConnectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var saver = (WalletConnect) target;

        if (GUILayout.Button("Clear Session"))
        {
            saver.CLearSession();
        }
    }
}

#endif