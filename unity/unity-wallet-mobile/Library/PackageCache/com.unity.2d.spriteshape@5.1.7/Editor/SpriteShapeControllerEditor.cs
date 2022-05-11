using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;
using UnityEditor;
using UnityEditor.U2D.SpriteShapeInternal;
using UnityEditor.U2D.Common;
using UnityEditor.AnimatedValues;
using UnityEditor.U2D.Path;
using UnityEngine.SceneManagement;

namespace UnityEditor.U2D
{

    [CustomEditor(typeof(SpriteShapeController))]
    [CanEditMultipleObjects]
    internal class SpriteShapeControllerEditor : PathComponentEditor<CustomPath>
    {
        private static class Contents
        {
            public static readonly GUIContent splineLabel = new GUIContent("Spline");
            public static readonly string editSplineLabel = "Edit Spline";
            public static readonly GUIContent fillLabel = new GUIContent("Fill");
            public static readonly GUIContent colliderLabel = new GUIContent("Collider");
            public static readonly GUIContent fillPixelPerUnitLabel = new GUIContent("Pixel Per Unit", "Pixel Per Unit for fill texture.");
            public static readonly GUIContent spriteShapeProfile = new GUIContent("Profile", "The SpriteShape Profile to render");
            public static readonly GUIContent materialLabel = new GUIContent("Material", "Material to be used by SpriteRenderer");
            public static readonly GUIContent colorLabel = new GUIContent("Color", "Rendering color for the Sprite graphic");
            public static readonly GUIContent metaDataLabel = new GUIContent("Meta Data", "SpriteShape specific controlpoint data");
            public static readonly GUIContent showComponentsLabel = new GUIContent("Show Render Stuff", "Show Renderer Components.");
            public static readonly GUIContent[] splineDetailOptions = { new GUIContent("High Quality"), new GUIContent("Medium Quality"), new GUIContent("Low Quality") };
            public static readonly GUIContent splineDetail = new GUIContent("Detail", "Tessellation Quality for rendering.");
            public static readonly GUIContent openEndedLabel = new GUIContent("Open Ended", "Is the path open ended or closed.");
            public static readonly GUIContent adaptiveUVLabel = new GUIContent("Adaptive UV", "Allow Adaptive UV Generation");
            public static readonly GUIContent enableTangentsLabel = new GUIContent("Enable Tangents", "Enable Tangents for 2D Lighting.");
            public static readonly GUIContent worldUVLabel = new GUIContent("Worldspace UV", "Generate UV for world space.");
            public static readonly GUIContent stretchUVLabel = new GUIContent("Stretch UV", "Stretch the Fill UV to full Rect.");
            public static readonly GUIContent stretchTilingLabel = new GUIContent("Stretch Tiling", "Stretch Tiling Count.");
            public static readonly GUIContent colliderDetail = new GUIContent("Detail", "Tessellation Quality on the collider.");
            public static readonly GUIContent cornerThresholdDetail = new GUIContent("Corner Threshold", "Corner angle threshold below which corners wont be placed.");
            public static readonly GUIContent colliderOffset = new GUIContent("Offset", "Extrude collider distance.");
            public static readonly GUIContent updateColliderLabel = new GUIContent("Update Collider", "Update Collider as you edit SpriteShape");
            public static readonly GUIContent optimizeColliderLabel = new GUIContent("Optimize Collider", "Cleanup planar self-intersections and optimize collider points");
            public static readonly GUIContent optimizeGeometryLabel = new GUIContent("Optimize Geometry", "Simplify geometry");
            public static readonly GUIContent cacheGeometryLabel = new GUIContent("Cache Geometry", "Bake geometry data. This will save geometry data on editor and load it on runtime instead of generating.");
            public static readonly GUIContent uTess2DLabel = new GUIContent("Fill Tessellation (C# Job)", "Use C# Jobs to generate Fill Geometry. (Edge geometry always uses C# Jobs)");
        }

        private SerializedProperty m_SpriteShapeProp;
        private SerializedProperty m_SplineDetailProp;
        private SerializedProperty m_IsOpenEndedProp;
        private SerializedProperty m_AdaptiveUVProp;
        private SerializedProperty m_StretchUVProp;
        private SerializedProperty m_StretchTilingProp;
        private SerializedProperty m_WorldSpaceUVProp;
        private SerializedProperty m_FillPixelPerUnitProp;
        private SerializedProperty m_CornerAngleThresholdProp;

        private SerializedProperty m_ColliderAutoUpdate;
        private SerializedProperty m_ColliderDetailProp;
        private SerializedProperty m_ColliderOffsetProp;

        private SerializedProperty m_OptimizeColliderProp;
        private SerializedProperty m_OptimizeGeometryProp;
        private SerializedProperty m_EnableTangentsProp;
        private SerializedProperty m_GeometryCachedProp;
        private SerializedProperty m_UTess2DGeometryProp;
        
        private int m_CollidersCount = 0;
        private int[] m_QualityValues = new int[] { (int)QualityDetail.High, (int)QualityDetail.Mid, (int)QualityDetail.Low };
        readonly AnimBool m_ShowStretchOption = new AnimBool();
        readonly AnimBool m_ShowNonStretchOption = new AnimBool();
        private struct ShapeSegment
        {
            public int start;
            public int end;
            public int angleRange;
        };

        private struct ShapeAngleRange
        {
            public float start;
            public float end;
            public int order;
            public int index;
        };

        int m_SelectedPoint = -1;
        int m_SelectedAngleRange = -1;
        int m_SpriteShapeHashCode = 0;
        int m_SplineHashCode = 0;
        List<ShapeSegment> m_ShapeSegments = new List<ShapeSegment>();
        SpriteSelector spriteSelector = new SpriteSelector();

        private SpriteShapeController m_SpriteShapeController 
        {
            get { return target as SpriteShapeController; } 
        }

        private void OnEnable()
        {
            if (target == null)
                return;

            m_SpriteShapeProp = serializedObject.FindProperty("m_SpriteShape");
            m_SplineDetailProp = serializedObject.FindProperty("m_SplineDetail");
            m_IsOpenEndedProp = serializedObject.FindProperty("m_Spline").FindPropertyRelative("m_IsOpenEnded");
            m_AdaptiveUVProp = serializedObject.FindProperty("m_AdaptiveUV");
            m_StretchUVProp = serializedObject.FindProperty("m_StretchUV");
            m_StretchTilingProp = serializedObject.FindProperty("m_StretchTiling");
            m_WorldSpaceUVProp = serializedObject.FindProperty("m_WorldSpaceUV");
            m_FillPixelPerUnitProp = serializedObject.FindProperty("m_FillPixelPerUnit");
            m_CornerAngleThresholdProp = serializedObject.FindProperty("m_CornerAngleThreshold");

            m_ColliderAutoUpdate = serializedObject.FindProperty("m_UpdateCollider");
            m_ColliderDetailProp = serializedObject.FindProperty("m_ColliderDetail");
            m_ColliderOffsetProp = serializedObject.FindProperty("m_ColliderOffset");
            m_OptimizeColliderProp = serializedObject.FindProperty("m_OptimizeCollider");
            m_OptimizeGeometryProp = serializedObject.FindProperty("m_OptimizeGeometry");
            m_EnableTangentsProp = serializedObject.FindProperty("m_EnableTangents");
            m_GeometryCachedProp = serializedObject.FindProperty("m_GeometryCached");
            m_UTess2DGeometryProp = serializedObject.FindProperty("m_UTess2D");

            m_ShowStretchOption.valueChanged.AddListener(Repaint);
            m_ShowStretchOption.value = ShouldShowStretchOption();

            m_ShowNonStretchOption.valueChanged.AddListener(Repaint);
            m_ShowNonStretchOption.value = !ShouldShowStretchOption();

            m_CollidersCount += ((m_SpriteShapeController.edgeCollider != null) ? 1 : 0);
            m_CollidersCount += ((m_SpriteShapeController.polygonCollider != null) ? 1 : 0);
        }

        private bool OnCollidersAddedOrRemoved()
        {
            PolygonCollider2D polygonCollider = m_SpriteShapeController.polygonCollider;
            EdgeCollider2D edgeCollider = m_SpriteShapeController.edgeCollider;
            int collidersCount = 0;

            if (polygonCollider != null)
                collidersCount = collidersCount + 1;
            if (edgeCollider != null)
                collidersCount = collidersCount + 1;

            if (collidersCount != m_CollidersCount)
            {
                m_CollidersCount = collidersCount;
                return true;
            }
            return false;
        }

        public void DrawHeader(GUIContent content)
        {
            EditorGUILayout.LabelField(content, EditorStyles.boldLabel);
        }

        private bool ShouldShowStretchOption()
        {
            return m_StretchUVProp.boolValue;
        }

        static bool WithinRange(ShapeAngleRange angleRange, float inputAngle)
        {
            float range = angleRange.end - angleRange.start;
            float angle = Mathf.Repeat(inputAngle - angleRange.start, 360f);
            angle = (angle == 360.0f) ? 0 : angle;
            return (angle >= 0f && angle <= range);
        }

        static int RangeFromAngle(List<ShapeAngleRange> angleRanges, float angle)
        {
            foreach (var range in angleRanges)
            {
                if (WithinRange(range, angle))
                    return range.index;
            }

            return -1;
        }

        private List<ShapeAngleRange> GetAngleRangeSorted(UnityEngine.U2D.SpriteShape ss)
        {
            List<ShapeAngleRange> angleRanges = new List<ShapeAngleRange>();
            int i = 0;
            foreach (var angleRange in ss.angleRanges)
            {
                ShapeAngleRange sar = new ShapeAngleRange() { start = angleRange.start, end = angleRange.end, order = angleRange.order, index = i };
                angleRanges.Add(sar);
                i++;
            }
            angleRanges.Sort((a, b) => a.order.CompareTo(b.order));
            return angleRanges;
        }

        private void GenerateSegments(SpriteShapeController sc, List<ShapeAngleRange> angleRanges)
        {
            var controlPointCount = sc.spline.GetPointCount();
            var angleRangeIndices = new int[controlPointCount];
            ShapeSegment activeSegment = new ShapeSegment() { start = -1, end = -1, angleRange = -1 };
            m_ShapeSegments.Clear();

            for (int i = 0; i < controlPointCount; ++i)
            {
                var actv = i;
                var next = SplineUtility.NextIndex(actv, controlPointCount);
                var pos1 = sc.spline.GetPosition(actv);
                var pos2 = sc.spline.GetPosition(next);
                bool continueStrip = (sc.spline.GetTangentMode(actv) == ShapeTangentMode.Continuous), edgeUpdated = false;
                float angle = 0;
                if (false == continueStrip || activeSegment.start == -1)
                    angle = SplineUtility.SlopeAngle(pos1, pos2) + 90.0f;

                next = (!sc.spline.isOpenEnded && next == 0) ? (actv + 1) : next;
                int mn = (actv < next) ? actv : next;
                int mx = (actv > next) ? actv : next;

                var anglerange = RangeFromAngle(angleRanges, angle);
                angleRangeIndices[actv] = anglerange;
                if (anglerange == -1)
                {
                    activeSegment = new ShapeSegment() { start = mn, end = mx, angleRange = anglerange };
                    m_ShapeSegments.Add(activeSegment);
                    continue;
                }

                // Check for Segments. Also check if the Segment Start has been resolved. Otherwise simply start with the next one
                if (activeSegment.start != -1)
                    continueStrip = continueStrip && (angleRangeIndices[activeSegment.start] != -1);

                bool canContinue = (actv != (controlPointCount - 1)) || (!sc.spline.isOpenEnded && (actv == (controlPointCount - 1)));
                if (continueStrip && canContinue)
                {
                    for (int s = 0; s < m_ShapeSegments.Count; ++s)
                    {
                        activeSegment = m_ShapeSegments[s];
                        if (activeSegment.start - mn == 1)
                        {
                            edgeUpdated = true;
                            activeSegment.start = mn;
                            m_ShapeSegments[s] = activeSegment;
                            break;
                        }
                        if (mx - activeSegment.end == 1)
                        {
                            edgeUpdated = true;
                            activeSegment.end = mx;
                            m_ShapeSegments[s] = activeSegment;
                            break;
                        }
                    }
                }

                if (!edgeUpdated)
                {
                    activeSegment.start = mn;
                    activeSegment.end = mx;
                    activeSegment.angleRange = anglerange;
                    m_ShapeSegments.Add(activeSegment);
                }

            }
        }

        private int GetAngleRange(SpriteShapeController sc, int point, ref int startPoint)
        {
            int angleRange = -1;
            startPoint = point;
            for (int i = 0; i < m_ShapeSegments.Count; ++i)
            {
                if (point >= m_ShapeSegments[i].start && point < m_ShapeSegments[i].end)
                {
                    angleRange = m_ShapeSegments[i].angleRange;
                    startPoint = point; //  m_ShapeSegments[i].start;
                    if (angleRange >= sc.spriteShape.angleRanges.Count)
                        angleRange = 0;
                    break;
                }
            }
            return angleRange;
        }

        private void UpdateSegments()
        {
            var sc = target as SpriteShapeController;

            // Either SpriteShape Asset or SpriteShape Data has changed. 
            if (m_SpriteShapeHashCode != sc.spriteShapeHashCode || m_SplineHashCode != sc.splineHashCode)
            {
                List<ShapeAngleRange> angleRanges = GetAngleRangeSorted(sc.spriteShape);
                GenerateSegments(sc, angleRanges);
                m_SpriteShapeHashCode = sc.spriteShapeHashCode;
                m_SplineHashCode = sc.splineHashCode;
                m_SelectedPoint = -1;
            }
        }

        private int ResolveSpriteIndex(List<int> spriteIndices, ISelection<int> selection, ref List<int> startPoints)
        {
            var spriteIndexValue = spriteIndices.FirstOrDefault();
            var sc = target as SpriteShapeController;
            var spline = sc.spline;

            if (sc == null || sc.spriteShape == null)
                return -1;
            UpdateSegments();

            if (sc.spriteShape != null)
            {
                if (selection.Count == 1)
                {
                    m_SelectedAngleRange = GetAngleRange(sc, selection.elements[0], ref m_SelectedPoint);
                    startPoints.Add(m_SelectedPoint);
                    spriteIndexValue = spline.GetSpriteIndex(m_SelectedPoint);
                }
                else
                {
                    m_SelectedAngleRange = -1;
                    foreach (var index in selection.elements)
                    {
                        int startPoint = index;
                        int angleRange = GetAngleRange(sc, index, ref startPoint);
                        if (m_SelectedAngleRange != -1 && angleRange != m_SelectedAngleRange)
                        {
                            m_SelectedAngleRange = -1;
                            break;
                        }
                        startPoints.Add(startPoint);
                        m_SelectedAngleRange = angleRange;
                    }
                }
            }

            if (m_SelectedAngleRange != -1)
                spriteSelector.UpdateSprites(sc.spriteShape.angleRanges[m_SelectedAngleRange].sprites.ToArray());
            else
                spriteIndexValue = -1;
            return spriteIndexValue;
        }

        public int GetAngleRange(int index)
        {
            int startPoint = 0;
            var sc = target as SpriteShapeController;
            UpdateSegments();
            return GetAngleRange(sc, index, ref startPoint);
        }

        public override void OnInspectorGUI()
        {
            var updateCollider = false;
            EditorGUI.BeginChangeCheck();

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_SpriteShapeProp, Contents.spriteShapeProfile);

            var hasEditToolChanged = DoEditButton<SpriteShapeEditorTool>(PathEditorToolContents.icon, Contents.editSplineLabel);
            if (hasEditToolChanged && !UnityEditor.EditorTools.ToolManager.activeToolType.Equals(typeof(SpriteShapeEditorTool)))
                SpriteShapeUpdateCache.UpdateCache(targets);

            DoPathInspector<SpriteShapeEditorTool>();
            var pathTool = SpriteShapeEditorTool.activeSpriteShapeEditorTool;

            if (Selection.gameObjects.Length == 1 && pathTool != null)
            {

                var sc = target as SpriteShapeController;
                var path = pathTool.GetPath(sc);
                if (path != null)
                {

                    var selection = path.selection;

                    if (selection.Count > 0)
                    {

                        var spline = sc.spline;
                        var spriteIndices = new List<int>();

                        List<int> startPoints = new List<int>();
                        foreach (int index in selection.elements)
                            spriteIndices.Add(spline.GetSpriteIndex(index));

                        var spriteIndexValue = ResolveSpriteIndex(spriteIndices, selection, ref startPoints);
                        if (spriteIndexValue != -1)
                        {
                            EditorGUI.BeginChangeCheck();

                            spriteSelector.ShowGUI(spriteIndexValue);

                            if (EditorGUI.EndChangeCheck())
                            {
                                foreach (var index in startPoints)
                                {
                                    var data = path.GetData(index);
                                    data.spriteIndex = spriteSelector.selectedIndex;
                                    path.SetData(index, data);
                                }
                                pathTool.SetPath(target);
                            }
                        }
                        EditorGUILayout.Space();
                    }
                }
            }

            DoSnappingInspector<SpriteShapeEditorTool>();

            EditorGUILayout.Space();
            DrawHeader(Contents.splineLabel);

            EditorGUILayout.IntPopup(m_SplineDetailProp, Contents.splineDetailOptions, m_QualityValues, Contents.splineDetail);
            serializedObject.ApplyModifiedProperties();

            DoOpenEndedInspector<SpriteShapeEditorTool>(m_IsOpenEndedProp);

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_AdaptiveUVProp, Contents.adaptiveUVLabel);
            if (!m_IsOpenEndedProp.boolValue)
                EditorGUILayout.PropertyField(m_OptimizeGeometryProp, Contents.optimizeGeometryLabel);
            EditorGUILayout.PropertyField(m_EnableTangentsProp, Contents.enableTangentsLabel);

            if (UnityEditor.EditorTools.ToolManager.activeToolType == typeof(SpriteShapeEditorTool))
            {
                // Cache Geometry is only editable for Scene Objects or when in Prefab Isolation Mode. 
                if (Selection.gameObjects.Length == 1 && Selection.transforms.Contains(Selection.gameObjects[0].transform))
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(m_GeometryCachedProp, Contents.cacheGeometryLabel);
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (m_GeometryCachedProp.boolValue)
                        {
                            var geometryCache = m_SpriteShapeController.spriteShapeGeometryCache;
                            if (!geometryCache)
                                geometryCache = m_SpriteShapeController.gameObject
                                    .AddComponent<SpriteShapeGeometryCache>();
                            geometryCache.hideFlags = HideFlags.HideInInspector;
                        }
                        else
                        {
                            if (m_SpriteShapeController.spriteShapeGeometryCache)
                                Object.DestroyImmediate(m_SpriteShapeController.spriteShapeGeometryCache);
                        }

                        m_SpriteShapeController.RefreshSpriteShape();
                    }
                }
                SpriteShapeUpdateCache.s_cacheGeometrySet = true;
            }

            EditorGUI.BeginChangeCheck();
            var threshold = EditorGUILayout.Slider(Contents.cornerThresholdDetail, m_CornerAngleThresholdProp.floatValue, 0.0f, 90.0f);
            if (EditorGUI.EndChangeCheck())
            { 
                m_CornerAngleThresholdProp.floatValue = threshold;
                updateCollider = true;
            }
            
            EditorGUILayout.Space();
            DrawHeader(Contents.fillLabel);
            EditorGUILayout.PropertyField(m_UTess2DGeometryProp, Contents.uTess2DLabel);
            EditorGUILayout.PropertyField(m_StretchUVProp, Contents.stretchUVLabel);

            if (ShouldShowStretchOption())
            {
                EditorGUILayout.PropertyField(m_StretchTilingProp, Contents.stretchTilingLabel);
            }
            else
            {
                EditorGUILayout.PropertyField(m_FillPixelPerUnitProp, Contents.fillPixelPerUnitLabel);
                EditorGUILayout.PropertyField(m_WorldSpaceUVProp, Contents.worldUVLabel);
            }

            if (m_SpriteShapeController.gameObject.GetComponent<PolygonCollider2D>() != null || m_SpriteShapeController.gameObject.GetComponent<EdgeCollider2D>() != null)
            {
                EditorGUILayout.Space();
                DrawHeader(Contents.colliderLabel);
                EditorGUILayout.PropertyField(m_ColliderAutoUpdate, Contents.updateColliderLabel);
                if (m_ColliderAutoUpdate.boolValue)
                { 
                    EditorGUILayout.PropertyField(m_ColliderOffsetProp, Contents.colliderOffset);
                    EditorGUILayout.PropertyField(m_OptimizeColliderProp, Contents.optimizeColliderLabel);
                    if (m_OptimizeColliderProp.boolValue)
                        EditorGUILayout.IntPopup(m_ColliderDetailProp, Contents.splineDetailOptions, m_QualityValues, Contents.colliderDetail);
                }
            }
            if (EditorGUI.EndChangeCheck())
            { 
                updateCollider = true;
            }

            serializedObject.ApplyModifiedProperties();

            if (updateCollider || OnCollidersAddedOrRemoved())
                BakeCollider();
        }

        void BakeCollider()
        {
            if (m_SpriteShapeController.autoUpdateCollider == false)
                return;

            PolygonCollider2D polygonCollider = m_SpriteShapeController.polygonCollider;
            if (polygonCollider)
            {
                Undo.RegisterCompleteObjectUndo(polygonCollider, Undo.GetCurrentGroupName());
                EditorUtility.SetDirty(polygonCollider);
                m_SpriteShapeController.RefreshSpriteShape();
            }

            EdgeCollider2D edgeCollider = m_SpriteShapeController.edgeCollider;
            if (edgeCollider)
            {
                Undo.RegisterCompleteObjectUndo(edgeCollider, Undo.GetCurrentGroupName());
                EditorUtility.SetDirty(edgeCollider);
                m_SpriteShapeController.RefreshSpriteShape();

            }
        }

        void ShowMaterials(bool show)
        {
            HideFlags hideFlags = HideFlags.HideInInspector;

            if (show)
                hideFlags = HideFlags.None;

            Material[] materials = m_SpriteShapeController.spriteShapeRenderer.sharedMaterials;

            foreach (Material material in materials)
            {
                material.hideFlags = hideFlags;
                EditorUtility.SetDirty(material);
            }
        }

        [DrawGizmo(GizmoType.InSelectionHierarchy)]
        static void RenderSpline(SpriteShapeController m_SpriteShapeController, GizmoType gizmoType)
        {
            if (UnityEditor.EditorTools.ToolManager.activeToolType == typeof(SpriteShapeEditorTool))
                return;

            var m_Spline = m_SpriteShapeController.spline;
            var oldMatrix = Handles.matrix;
            var oldColor = Handles.color;
            Handles.matrix = m_SpriteShapeController.transform.localToWorldMatrix;
            Handles.color = Color.grey;
            var pointCount = m_Spline.GetPointCount();
            for (var i = 0; i < (m_Spline.isOpenEnded ? pointCount - 1 : pointCount); ++i)
            {
                Vector3 p1 = m_Spline.GetPosition(i);
                Vector3 p2 = m_Spline.GetPosition((i + 1) % pointCount);
                var t1 = p1 + m_Spline.GetRightTangent(i);
                var t2 = p2 + m_Spline.GetLeftTangent((i + 1) % pointCount);
                Vector3[] bezierPoints = Handles.MakeBezierPoints(p1, p2, t1, t2, m_SpriteShapeController.splineDetail);
                Handles.DrawAAPolyLine(bezierPoints);
            }
            Handles.matrix = oldMatrix;
            Handles.color = oldColor;
        }
    }

    [UnityEditor.InitializeOnLoad]
    internal static class SpriteShapeUpdateCache
    {

        internal static bool s_cacheGeometrySet = false;
        static SpriteShapeUpdateCache()
        {
            UnityEditor.EditorApplication.playModeStateChanged += change =>
            {
                if (change == UnityEditor.PlayModeStateChange.ExitingEditMode)
                    UpdateSpriteShapeCacheInOpenScenes();
            };
        }

        static void UpdateSpriteShapeCacheInOpenScenes()
        {
            for (int i = 0; s_cacheGeometrySet && (i < SceneManager.sceneCount); ++i)
            {
                var scene = SceneManager.GetSceneAt(i);
                var gos = scene.GetRootGameObjects();
                foreach (var go in gos)
                {
                    var scs = go.GetComponentsInChildren<SpriteShapeController>();
                    foreach (var sc in scs)
                        if (sc.spriteShapeGeometryCache)
                            sc.spriteShapeGeometryCache.UpdateGeometryCache();
                }
            }

            s_cacheGeometrySet = false;
        }
        
        internal static void UpdateCache(UnityEngine.Object[] targets)
        {
            foreach (var t in targets)
            {
                var s = t as SpriteShapeController;
                if (s)
                    if (s.spriteShapeGeometryCache)
                        s.spriteShapeGeometryCache.UpdateGeometryCache();
            }
        }
    }

}
