using System;
using UnityEngine;
using UnityEngine.U2D;
using UnityEditor;
using UnityEditor.U2D.Path;
using UnityEditor.EditorTools;
using UnityEditor.ShortcutManagement;
using UnityEditor.U2D.Common;

namespace UnityEditor.U2D.SpriteShapeInternal
{
    internal class CustomDrawer : IDrawer
    {
        private IDrawer m_Drawer = new Drawer();
        private SpriteShapeController m_SpriteShapeController;

        public CustomDrawer(SpriteShapeController spriteShapeController)
        {
            m_SpriteShapeController = spriteShapeController;
        }

        private int GetSubDivisionCount()
        {
            return m_SpriteShapeController.splineDetail;
        }

        void IDrawer.DrawBezier(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float width, Color color)
        {
            Handles.color = color;
            Handles.DrawAAPolyLine(null, width, Handles.MakeBezierPoints(p1, p4, p2, p3, GetSubDivisionCount()));
        }

        void IDrawer.DrawCreatePointPreview(Vector3 position)
        {
            m_Drawer.DrawCreatePointPreview(position);
        }

        void IDrawer.DrawLine(Vector3 p1, Vector3 p2, float width, Color color)
        {
            m_Drawer.DrawLine(p1, p2, width, color);
        }

        void IDrawer.DrawPoint(Vector3 position)
        {
            m_Drawer.DrawPoint(position);
        }

        void IDrawer.DrawPointHovered(Vector3 position)
        {
            m_Drawer.DrawPointHovered(position);
        }

        void IDrawer.DrawPointSelected(Vector3 position)
        {
            m_Drawer.DrawPointSelected(position);
        }

        void IDrawer.DrawTangent(Vector3 position, Vector3 tangent)
        {
            m_Drawer.DrawTangent(position, tangent);
        }
    }

    [Serializable]
    internal class SpriteShapeData
    {
        public float height = 1f;
        public int spriteIndex;
        public int corner = 1;
        public int cornerMode = 1;
    }

    internal class ScriptableSpriteShapeData : ScriptableData<SpriteShapeData> { }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(ScriptableSpriteShapeData))]
    internal class SpriteShapeDataInspector : Editor
    {
        private static class Contents
        {
            public static readonly GUIContent heightLabel = new GUIContent("Height", "Height override for control point.");
            public static readonly GUIContent cornerLabel = new GUIContent("Corner", "Set if Corner is automatic or disabled.");

            public static readonly int[] cornerValues = { 0, 1, 2 };
            public static readonly GUIContent[] cornerOptions = { new GUIContent("Disabled"), new GUIContent("Automatic"), new GUIContent("Stretched"),  };
        }

        private SerializedProperty m_Data;
        private SerializedProperty m_HeightProperty;
        private SerializedProperty m_CornerProperty;
        private SerializedProperty m_CornerModeProperty;

        private void OnEnable()
        {
            m_Data = serializedObject.FindProperty("m_Data");
            m_HeightProperty = m_Data.FindPropertyRelative("height");
            m_CornerProperty = m_Data.FindPropertyRelative("corner");
            m_CornerModeProperty = m_Data.FindPropertyRelative("cornerMode");
        }

        internal void OnHeightChanged(UnityEngine.Object[] targets, float height)
        {
            foreach (var t in targets)
            {
                var shapeData = t as ScriptableSpriteShapeData;
                var path = SpriteShapeEditorTool.activeSpriteShapeEditorTool.GetPath(shapeData.owner);

                if (path.selection.Count == 0)
                    continue;

                path.undoObject.RegisterUndo("Set Height");

                for (var i = 0; i < path.pointCount; ++i)
                {
                    if (!path.selection.Contains(i))
                        continue;

                    var data = path.data[i] as SpriteShapeData;
                    data.height = height;
                }

                SpriteShapeEditorTool.activeSpriteShapeEditorTool.SetPath(shapeData.owner);
            }
        }

        internal void OnCornerModeChanged(UnityEngine.Object[] targets, int cornerValue)
        {
            foreach (var t in targets)
            {
                var shapeData = t as ScriptableSpriteShapeData;
                var path = SpriteShapeEditorTool.activeSpriteShapeEditorTool.GetPath(shapeData.owner);

                if (path.selection.Count == 0)
                    continue;

                path.undoObject.RegisterUndo("Set Corner Mode");

                for (var i = 0; i < path.pointCount; ++i)
                {
                    if (!path.selection.Contains(i))
                        continue;

                    var data = path.data[i] as SpriteShapeData;
                    data.cornerMode = cornerValue;
                    data.corner = cornerValue != 0 ? 1 : 0;
                }

                SpriteShapeEditorTool.activeSpriteShapeEditorTool.SetPath(shapeData.owner);
            }
        }

        public override void OnInspectorGUI()
        { 
            var scriptableSpriteShapeData = target as ScriptableSpriteShapeData;
            if (!scriptableSpriteShapeData)
                return;
            
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            var heightValue = EditorGUILayout.Slider(Contents.heightLabel, m_HeightProperty.floatValue, 0.1f, 4.0f);
            if (EditorGUI.EndChangeCheck())
            {
                m_HeightProperty.floatValue = heightValue;
                if (serializedObject.isEditingMultipleObjects)
                    OnHeightChanged(targets, heightValue);
            }

            EditorGUI.BeginChangeCheck();
            var cornerValue = EditorGUILayout.IntPopup(Contents.cornerLabel, m_CornerModeProperty.intValue, Contents.cornerOptions, Contents.cornerValues);
            if (EditorGUI.EndChangeCheck())
            {
                m_CornerModeProperty.intValue = cornerValue;
                m_CornerProperty.intValue = cornerValue != 0 ? 1 : 0;
                if (serializedObject.isEditingMultipleObjects)
                    OnCornerModeChanged(targets, cornerValue);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

    internal class CustomPath : GenericScriptablePath<SpriteShapeData> { }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(CustomPath))]
    internal class CustomPathInspector : GenericScriptablePathInspector<ScriptableSpriteShapeData, SpriteShapeData> { }

    [EditorTool("Edit SpriteShape", typeof(SpriteShapeController))]
    internal class SpriteShapeEditorTool : PathEditorTool<CustomPath>
    {
        private static InternalEditorBridge.ShortcutContext m_ShortcutContext;

        public static SpriteShapeEditorTool activeSpriteShapeEditorTool
        {
            get
            {
                if (m_ShortcutContext != null)
                    return m_ShortcutContext.context as SpriteShapeEditorTool;
                return null;
            }
        }

        protected override bool GetLinearTangentIsZero(UnityEngine.Object target)
        {
            return true;
        }

        protected override IDrawer GetCustomDrawer(UnityEngine.Object target)
        {
            return new CustomDrawer(target as SpriteShapeController);
        }

        protected override IShape GetShape(UnityEngine.Object target)
        {
            return Polygon.empty;
        }

        protected override void Initialize(CustomPath shapeEditor, SerializedObject serializedObject)
        {
            var controller = serializedObject.targetObject as SpriteShapeController;
            var spline = controller.spline;
            shapeEditor.shapeType = ShapeType.Spline;
            shapeEditor.isOpenEnded = spline.isOpenEnded;

            for (var i = 0; i < spline.GetPointCount(); ++i)
            {
                var position = spline.GetPosition(i);

                shapeEditor.AddPoint(new ControlPoint()
                {
                    position = spline.GetPosition(i),
                    localLeftTangent = spline.GetLeftTangent(i),
                    localRightTangent = spline.GetRightTangent(i),
                    tangentMode = (TangentMode)spline.GetTangentMode(i)
                });

                shapeEditor.SetData(i, new SpriteShapeData()
                {
                    spriteIndex = spline.GetSpriteIndex(i),
                    height = spline.GetHeight(i),
                    cornerMode = (int)spline.GetCornerMode(i),
                    corner = spline.GetCorner(i) ? 1 : 0,
                });
            }

            shapeEditor.UpdateTangentsFromMode();
        }

        protected override void SetShape(CustomPath shapeEditor, SerializedObject serializedObject)
        {
            serializedObject.Update();

            var controller = serializedObject.targetObject as SpriteShapeController;
            var splineProp = serializedObject.FindProperty("m_Spline");
            var controlPointsProp = splineProp.FindPropertyRelative("m_ControlPoints");
            
            splineProp.FindPropertyRelative("m_IsOpenEnded").boolValue = shapeEditor.isOpenEnded;

            controlPointsProp.arraySize = shapeEditor.pointCount;

            for (var i = 0; i < shapeEditor.pointCount; ++i)
            {
                var elementProp = controlPointsProp.GetArrayElementAtIndex(i);
                var point = shapeEditor.GetPoint(i);
                var data = shapeEditor.GetData(i);

                elementProp.FindPropertyRelative("position").vector3Value = point.position;
                elementProp.FindPropertyRelative("leftTangent").vector3Value = point.localLeftTangent;
                elementProp.FindPropertyRelative("rightTangent").vector3Value = point.localRightTangent;
                elementProp.FindPropertyRelative("mode").enumValueIndex = (int)point.tangentMode;
                elementProp.FindPropertyRelative("height").floatValue = data.height;
                elementProp.FindPropertyRelative("spriteIndex").intValue = data.spriteIndex;
                elementProp.FindPropertyRelative("corner").intValue = data.corner;
                elementProp.FindPropertyRelative("m_CornerMode").intValue = data.cornerMode;
            }

            serializedObject.ApplyModifiedProperties();
        }

        protected override void OnActivate()
        {
            RegisterShortcuts();
        }

        protected override void OnDeactivate()
        {
            UnregisterShortcuts();
        }
        
        private void CycleSpriteIndex()
        {
            foreach(var target in targets)
            {
                var spriteShapeController = target as SpriteShapeController;
                var shapeEditor = GetPath(target);
                Editor cachedEditor = null;
                Editor.CreateCachedEditor(spriteShapeController, typeof(SpriteShapeControllerEditor), ref cachedEditor);
                if (spriteShapeController == null || spriteShapeController.spriteShape == null || shapeEditor.selection.Count == 0 || cachedEditor == null || shapeEditor.selection.Count > 1)
                    continue;

                var spriteShape = spriteShapeController.spriteShape;
                var scEditor = cachedEditor as SpriteShapeControllerEditor;
                var selected = shapeEditor.selection.elements[0];

                if (shapeEditor.selection.Contains(selected))
                {
                    shapeEditor.undoObject.RegisterUndo("Cycle Variant");
                    var data = shapeEditor.GetData(selected);

                    var angleRangeIndex = scEditor.GetAngleRange(selected);
                    if (angleRangeIndex != -1)
                    {
                        var angleRange = spriteShape.angleRanges[angleRangeIndex];
                        if (angleRange.sprites.Count > 0)
                            data.spriteIndex = (data.spriteIndex + 1) % angleRange.sprites.Count;
                        shapeEditor.SetData(selected, data);
                    }

                    SetPath(target);
                }
            }
        }

        private static float SlopeAngle(Vector2 start, Vector2 end)
        {
            var dir = start - end;
            dir.Normalize();
            var dvup = new Vector2(0, 1f);
            var dvrt = new Vector2(1f, 0);

            var dr = Vector2.Dot(dir, dvrt);
            var du = Vector2.Dot(dir, dvup);
            var cu = Mathf.Acos(du);
            var sn = dr >= 0 ? 1.0f : -1.0f;
            var an = cu * Mathf.Rad2Deg * sn;

            // Adjust angles when direction is parallel to Up Axis.
            an = (du != 1f) ? an : 0;
            an = (du != -1f) ? an : -180f;
            return an;
        }

        private void RegisterShortcuts()
        {
            m_ShortcutContext = new InternalEditorBridge.ShortcutContext()
            {
                isActive = () => GUIUtility.hotControl == 0,
                context = this
            };

            InternalEditorBridge.RegisterShortcutContext(m_ShortcutContext);
        }

        private void UnregisterShortcuts()
        {
            InternalEditorBridge.UnregisterShortcutContext(m_ShortcutContext);
        }

        [Shortcut("SpriteShape Editing/Cycle Tangent Mode", typeof(InternalEditorBridge.ShortcutContext), KeyCode.M)]
        private static void ShortcutCycleTangentMode(ShortcutArguments args)
        {
            if (args.context == m_ShortcutContext)
                (m_ShortcutContext.context as SpriteShapeEditorTool).CycleTangentMode();
        }

        [Shortcut("SpriteShape Editing/Cycle Variant", typeof(InternalEditorBridge.ShortcutContext), KeyCode.N)]
        private static void ShortcutCycleSpriteIndex(ShortcutArguments args)
        {
            if (args.context == m_ShortcutContext)
                (m_ShortcutContext.context as SpriteShapeEditorTool).CycleSpriteIndex();
        }

        [Shortcut("SpriteShape Editing/Mirror Tangent", typeof(InternalEditorBridge.ShortcutContext), KeyCode.B)]
        private static void ShortcutCycleMirrorTangent(ShortcutArguments args)
        {
            if (args.context == m_ShortcutContext)
                (m_ShortcutContext.context as SpriteShapeEditorTool).MirrorTangent();
        }
    }
}
