#if ENABLE_ANIMATION_COLLECTION && ENABLE_ANIMATION_BURST
#define ENABLE_ANIMATION_PERFORMANCE
#endif

using UnityEngine;
using UnityEditorInternal;
using UnityEngine.U2D.Animation;
using UnityEditor.IMGUI.Controls;
using UnityEngine.U2D;
using UnityEngine.U2D.Common;

namespace UnityEditor.U2D.Animation
{
    [CustomEditor(typeof(SpriteSkin))]
    [CanEditMultipleObjects]
    class SpriteSkinEditor : Editor
    {
        private static class Contents
        {
            public static readonly GUIContent listHeaderLabel = new GUIContent("Bones", "GameObject Transform to represent the Bones defined by the Sprite that is currently used for deformation.");
            public static readonly GUIContent rootBoneLabel = new GUIContent("Root Bone", "GameObject Transform to represent the Root Bone.");
            public static readonly string spriteNotFound = L10n.Tr("Sprite not found in SpriteRenderer");
            public static readonly string spriteHasNoSkinningInformation = L10n.Tr("Sprite has no Bind Poses");
            public static readonly string spriteHasNoWeights = L10n.Tr("Sprite has no weights");
            public static readonly string rootTransformNotFound = L10n.Tr("Root Bone not set");
            public static readonly string rootTransformNotFoundInArray = L10n.Tr("Bone list doesn't contain a reference to the Root Bone");
            public static readonly string invalidTransformArray = L10n.Tr("Bone list is invalid");
            public static readonly string transformArrayContainsNull = L10n.Tr("Bone list contains unassigned references");
            public static readonly string invalidTransformArrayLength = L10n.Tr("The number of Sprite's Bind Poses and the number of Transforms should match");
            public static readonly GUIContent useManager = new GUIContent("Enable batching", "When enabled, SpriteSkin deformation will be done in batch to improve performance.");
            public static readonly GUIContent alwaysUpdate = new GUIContent("Always Update", "Executes deformation of SpriteSkin even when the associated SpriteRenderer has been culled and is not visible.");
            public static readonly string experimental = L10n.Tr("Experimental");
        }

        private static Color s_BoundingBoxHandleColor = new Color(255, 255, 255, 150) / 255;

        private SerializedProperty m_RootBoneProperty;
        private SerializedProperty m_BoneTransformsProperty;
        private SerializedProperty m_AlwaysUpdateProperty;
        private SpriteSkin m_SpriteSkin;
        private ReorderableList m_ReorderableList;
        private Sprite m_CurrentSprite;
        private BoxBoundsHandle m_BoundsHandle = new BoxBoundsHandle();
        private bool m_NeedsRebind = false;
#if ENABLE_ANIMATION_PERFORMANCE
        private SerializedProperty m_UseBatching;
        private bool m_ExperimentalFold;
#endif
        private bool m_BoneFold = true;

        private void OnEnable()
        {
            m_SpriteSkin = (SpriteSkin)target;
            m_SpriteSkin.OnEditorEnable();

            m_RootBoneProperty = serializedObject.FindProperty("m_RootBone");
#if ENABLE_ANIMATION_PERFORMANCE
            m_UseBatching = serializedObject.FindProperty("m_UseBatching");
#endif
            m_BoneTransformsProperty = serializedObject.FindProperty("m_BoneTransforms");
            m_AlwaysUpdateProperty = serializedObject.FindProperty("m_AlwaysUpdate");

            m_CurrentSprite = m_SpriteSkin.spriteRenderer.sprite;
            m_BoundsHandle.axes = BoxBoundsHandle.Axes.X | BoxBoundsHandle.Axes.Y;
            m_BoundsHandle.SetColor(s_BoundingBoxHandleColor);

            SetupReorderableList();

            Undo.undoRedoPerformed += UndoRedoPerformed;
        }

        private void OnDestroy()
        {
            Undo.undoRedoPerformed -= UndoRedoPerformed;
        }

        private void UndoRedoPerformed()
        {
            m_CurrentSprite = m_SpriteSkin.spriteRenderer.sprite;
        }

        private void SetupReorderableList()
        {
            m_ReorderableList = new ReorderableList(serializedObject, m_BoneTransformsProperty, false, true, false, false);
            m_ReorderableList.headerHeight = 1.0f;
            m_ReorderableList.elementHeightCallback = (int index) =>
                {
                    return EditorGUIUtility.singleLineHeight + 6;
                };
            m_ReorderableList.drawElementCallback = (Rect rect, int index, bool isactive, bool isfocused) =>
                {
                    var content = GUIContent.none;

                    if (m_CurrentSprite != null)
                    {
                        var bones = m_CurrentSprite.GetBones();
                        if (index < bones.Length)
                            content = new GUIContent(bones[index].name);
                    }

                    rect.y += 2f;
                    rect.height = EditorGUIUtility.singleLineHeight;
                    SerializedProperty element = m_BoneTransformsProperty.GetArrayElementAtIndex(index);
                    EditorGUI.PropertyField(rect, element, content);
                };
        }

        private void InitializeBoneTransformArray()
        {
            if (m_CurrentSprite)
            {
                var elementCount = m_BoneTransformsProperty.arraySize;
                var bindPoses = m_CurrentSprite.GetBindPoses();

                if (elementCount != bindPoses.Length)
                {
                    m_BoneTransformsProperty.arraySize = bindPoses.Length;

                    for (int i = elementCount; i < m_BoneTransformsProperty.arraySize; ++i)
                        m_BoneTransformsProperty.GetArrayElementAtIndex(i).objectReferenceValue = null;

                    m_NeedsRebind = true;
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_AlwaysUpdateProperty, Contents.alwaysUpdate);

            var sprite = m_SpriteSkin.spriteRenderer.sprite;
            var spriteChanged = m_CurrentSprite != sprite;

            if (m_ReorderableList == null || spriteChanged)
            {
                m_CurrentSprite = sprite;
                InitializeBoneTransformArray();
                SetupReorderableList();
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_RootBoneProperty, Contents.rootBoneLabel);
            if (EditorGUI.EndChangeCheck())
            {
                m_NeedsRebind = true;
            }

            m_BoneFold = EditorGUILayout.Foldout(m_BoneFold, Contents.listHeaderLabel, true);
            if (m_BoneFold)
            {
                EditorGUILayout.Space();
                if (!serializedObject.isEditingMultipleObjects)
                {
                    EditorGUI.BeginDisabledGroup(m_SpriteSkin.rootBone == null);
                    m_ReorderableList.DoLayoutList();
                    EditorGUI.EndDisabledGroup();
                }
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            EditorGUI.BeginDisabledGroup(!EnableCreateBones());
            DoGenerateBonesButton();
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!EnableSetBindPose());
            DoResetBindPoseButton();
            EditorGUI.EndDisabledGroup();

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

#if ENABLE_ANIMATION_PERFORMANCE
            m_ExperimentalFold = EditorGUILayout.Foldout(m_ExperimentalFold, Contents.experimental, true);
            if (m_ExperimentalFold)
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_UseBatching, Contents.useManager);
                if (EditorGUI.EndChangeCheck())
                {
                    foreach (var obj in targets)
                    {
                        ((SpriteSkin)obj).UseBatching(m_UseBatching.boolValue);
                    }
                }
                EditorGUI.indentLevel--;
            }
#endif

            serializedObject.ApplyModifiedProperties();

            if (m_NeedsRebind)
                Rebind();

            if (spriteChanged && !m_SpriteSkin.ignoreNextSpriteChange)
            {
                ResetBounds(Undo.GetCurrentGroupName());
                m_SpriteSkin.ignoreNextSpriteChange = false;
            }

            DoValidationWarnings();
        }

        private void Rebind()
        {
            foreach (var t in targets)
            {
                var spriteSkin = t as SpriteSkin;

                if(spriteSkin.spriteRenderer.sprite == null || spriteSkin.rootBone == null)
                    continue;

                spriteSkin.Rebind();
                ResetBoundsIfNeeded(spriteSkin);
            }

            m_NeedsRebind = false;
        }

        private void ResetBounds(string undoName = "Reset Bounds")
        {
            foreach (var t in targets)
            {
                var spriteSkin = t as SpriteSkin;

                if (!spriteSkin.isValid)
                    continue;

                Undo.RegisterCompleteObjectUndo(spriteSkin, undoName);
                spriteSkin.CalculateBounds();

                EditorUtility.SetDirty(spriteSkin);
            }
        }

        private void ResetBoundsIfNeeded(SpriteSkin spriteSkin)
        {
            if (spriteSkin.isValid && spriteSkin.bounds == new Bounds())
                spriteSkin.CalculateBounds();
        }

        private bool EnableCreateBones()
        {
            foreach (var t in targets)
            {
                var spriteSkin = t as SpriteSkin;
                var sprite = spriteSkin.spriteRenderer.sprite;

                if (sprite != null && spriteSkin.rootBone == null)
                    return true;
            }
            return false;
        }

        private bool EnableSetBindPose()
        {
            return IsAnyTargetValid();
        }

        private bool IsAnyTargetValid()
        {
            foreach (var t in targets)
            {
                var spriteSkin = t as SpriteSkin;

                if (spriteSkin.isValid)
                    return true;
            }
            return false;
        }

        private void DoGenerateBonesButton()
        {
            if (GUILayout.Button("Create Bones", GUILayout.MaxWidth(125f)))
            {
                foreach (var t in targets)
                {
                    var spriteSkin = t as SpriteSkin;
                    var sprite = spriteSkin.spriteRenderer.sprite;

                    if (sprite == null || spriteSkin.rootBone != null)
                        continue;

                    Undo.RegisterCompleteObjectUndo(spriteSkin, "Create Bones");

                    spriteSkin.CreateBoneHierarchy();

                    foreach (var transform in spriteSkin.boneTransforms)
                        Undo.RegisterCreatedObjectUndo(transform.gameObject, "Create Bones");

                    ResetBoundsIfNeeded(spriteSkin);

                    EditorUtility.SetDirty(spriteSkin);
                }
                BoneGizmo.instance.boneGizmoController.OnSelectionChanged();
            }
        }

        private void DoResetBindPoseButton()
        {
            if (GUILayout.Button("Reset Bind Pose", GUILayout.MaxWidth(125f)))
            {
                foreach (var t in targets)
                {
                    var spriteSkin = t as SpriteSkin;

                    if (!spriteSkin.isValid)
                        continue;

                    Undo.RecordObjects(spriteSkin.boneTransforms, "Reset Bind Pose");
                    spriteSkin.ResetBindPose();
                }
            }
        }

        private void DoValidationWarnings()
        {
            EditorGUILayout.Space();

            bool preAppendObjectName = targets.Length > 1;

            foreach (var t in targets)
            {
                var spriteSkin = t as SpriteSkin;

                var validationResult = spriteSkin.Validate();

                if (validationResult == SpriteSkinValidationResult.Ready)
                    continue;

                var text = "";

                switch (validationResult)
                {
                    case SpriteSkinValidationResult.SpriteNotFound:
                        text = Contents.spriteNotFound;
                        break;
                    case SpriteSkinValidationResult.SpriteHasNoSkinningInformation:
                        text = Contents.spriteHasNoSkinningInformation;
                        break;
                    case SpriteSkinValidationResult.SpriteHasNoWeights:
                        text = Contents.spriteHasNoWeights;
                        break;
                    case SpriteSkinValidationResult.RootTransformNotFound:
                        text = Contents.rootTransformNotFound;
                        break;
                    case SpriteSkinValidationResult.RootNotFoundInTransformArray:
                        text = Contents.rootTransformNotFoundInArray;
                        break;
                    case SpriteSkinValidationResult.InvalidTransformArray:
                        text = Contents.invalidTransformArray;
                        break;
                    case SpriteSkinValidationResult.InvalidTransformArrayLength:
                        text = Contents.invalidTransformArrayLength;
                        break;
                    case SpriteSkinValidationResult.TransformArrayContainsNull:
                        text = Contents.transformArrayContainsNull;
                        break;
                }

                if (preAppendObjectName)
                    text = spriteSkin.name + ": " + text;

                EditorGUILayout.HelpBox(text, MessageType.Warning);
            }
        }
    }
}
