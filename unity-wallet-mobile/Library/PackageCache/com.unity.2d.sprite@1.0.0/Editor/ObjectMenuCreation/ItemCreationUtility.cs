using System;
using System.IO;
using UnityEngine;

namespace UnityEditor.U2D
{
    static internal class ItemCreationUtility
    {
        static internal Action<int, ProjectWindowCallback.EndNameEditAction, string, Texture2D, string> StartNewAssetNameEditingDelegate = ProjectWindowUtil.StartNameEditingIfProjectWindowExists;

        static public GameObject CreateGameObject(string name, MenuCommand menuCommand, params Type[] components)
        {
            var parent = menuCommand.context as GameObject;
            var newGO = ObjectFactory.CreateGameObject(name, components);
            newGO.name = name;
            Selection.activeObject = newGO;
            GOCreationCommands.Place(newGO, parent);
            if (EditorSettings.defaultBehaviorMode == EditorBehaviorMode.Mode2D)
            {
                var position = newGO.transform.position;
                position.z = 0;
                newGO.transform.position = position;
            }
            Undo.RegisterCreatedObjectUndo(newGO, string.Format("Create {0}", name));
            return newGO;
        }

        static public T CreateAssetObjectFromTemplate<T>(string sourcePath) where T : UnityEngine.Object
        {
            var assetSelectionPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            var isFolder = false;
            if (!string.IsNullOrEmpty(assetSelectionPath))
                isFolder = File.GetAttributes(assetSelectionPath).HasFlag(FileAttributes.Directory);
            var path = ProjectWindowUtil.GetActiveFolderPath();
            if (isFolder)
            {
                path = assetSelectionPath;
            }
            var fileName = Path.GetFileName(sourcePath);
            var destName = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(path, fileName));
            var icon = EditorGUIUtility.IconContent<T>().image as Texture2D;
            StartNewAssetNameEditing(sourcePath, destName, icon, ProjectBrowser.kAssetCreationInstanceID_ForNonExistingAssets);
            return Selection.activeObject as T;
        }

        static public T CreateAssetObject<T>(string name) where T : UnityEngine.Object
        {
            var assetSelectionPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            var isFolder = false;
            if (!string.IsNullOrEmpty(assetSelectionPath))
                isFolder = File.GetAttributes(assetSelectionPath).HasFlag(FileAttributes.Directory);
            var path = ProjectWindowUtil.GetActiveFolderPath();
            if (isFolder)
            {
                path = assetSelectionPath;
            }
            var destName = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(path, name));
            var newObject = Activator.CreateInstance<T>();
            var icon = EditorGUIUtility.IconContent<T>().image as Texture2D;
            StartNewAssetNameEditing(null, destName, icon, newObject.GetInstanceID());
            return Selection.activeObject as T;
        }

        static private void StartNewAssetNameEditing(string source, string dest, Texture2D icon, int instanceId)
        {
            var action = ScriptableObject.CreateInstance<CreateAssetEndNameEditAction>();
            StartNewAssetNameEditingDelegate(instanceId, action, dest, icon, source);
        }

        internal class CreateAssetEndNameEditAction : ProjectWindowCallback.EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                var uniqueName = AssetDatabase.GenerateUniqueAssetPath(pathName);
                if (instanceId == ProjectBrowser.kAssetCreationInstanceID_ForNonExistingAssets && !string.IsNullOrEmpty(resourceFile))
                {
                    AssetDatabase.CopyAsset(resourceFile, uniqueName);
                    instanceId = AssetDatabase.LoadMainAssetAtPath(uniqueName).GetInstanceID();
                }
                else
                {
                    var obj = EditorUtility.InstanceIDToObject(instanceId);
                    AssetDatabase.CreateAsset(obj, uniqueName);
                }
                ProjectWindowUtil.FrameObjectInProjectWindow(instanceId);
            }
        }
    }
}
