using System.Collections.Generic;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Collections.LowLevel.Unsafe;
#if UNITY_EDITOR
using UnityEditor.U2D;
#endif

namespace UnityEngine.U2D
{

    /// <summary>
    /// SpriteShapeController component contains Spline and SpriteShape Profile information that is used when generating SpriteShape geometry.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(SpriteShapeRenderer))]
    [DisallowMultipleComponent]
    [HelpURLAttribute("https://docs.unity3d.com/Packages/com.unity.2d.spriteshape@latest/index.html?subfolder=/manual/SSController.html")]
    public class SpriteShapeController : MonoBehaviour
    {
        // Internal Dataset.
        const float s_DistanceTolerance = 0.001f;

        // Cached Objects.
        SpriteShape m_ActiveSpriteShape;
        EdgeCollider2D m_EdgeCollider2D;
        PolygonCollider2D m_PolygonCollider2D;
        SpriteShapeRenderer m_SpriteShapeRenderer;
        SpriteShapeGeometryCache m_SpriteShapeGeometryCache;

        Sprite[] m_SpriteArray = new Sprite[0];
        Sprite[] m_EdgeSpriteArray = new Sprite[0];
        Sprite[] m_CornerSpriteArray = new Sprite[0];
        AngleRangeInfo[] m_AngleRangeInfoArray = new AngleRangeInfo[0];

        // Required for Generation.
        NativeArray<float2> m_ColliderData;
        NativeArray<Vector4> m_TangentData;

        // Renderer Stuff.
        bool m_DynamicOcclusionLocal;
        bool m_DynamicOcclusionOverriden;

        // Hash Check.
        int m_ActiveSplineHash = 0;
        int m_ActiveSpriteShapeHash = 0;
        int m_MaxArrayCount = 0;
        JobHandle m_JobHandle;
        SpriteShapeParameters m_ActiveShapeParameters;

        // Serialized Data.
        [SerializeField]
        Spline m_Spline = new Spline();
        [SerializeField]
        SpriteShape m_SpriteShape;

        [SerializeField]
        float m_FillPixelPerUnit = 100.0f;
        [SerializeField]
        float m_StretchTiling = 1.0f;
        [SerializeField]
        int m_SplineDetail;
        [SerializeField]
        bool m_AdaptiveUV;
        [SerializeField]
        bool m_StretchUV;
        [SerializeField]
        bool m_WorldSpaceUV;
        [SerializeField]
        float m_CornerAngleThreshold = 30.0f;
        [SerializeField]
        int m_ColliderDetail;
        [SerializeField, Range(-0.5f, 0.5f)]
        float m_ColliderOffset;
        [SerializeField]
        bool m_UpdateCollider = true;
        [SerializeField]
        bool m_OptimizeCollider = true;
        [SerializeField]
        bool m_OptimizeGeometry = true;
        [SerializeField]
        bool m_EnableTangents = false;
        [SerializeField]
        [HideInInspector]
        bool m_GeometryCached = false;
        [SerializeField] 
        bool m_UTess2D = false;
        
        #region GetSet

        internal int maxArrayCount
        {
            get { return m_MaxArrayCount; }
            set { m_MaxArrayCount = value; }
        }

        internal bool geometryCached
        {
            get { return m_GeometryCached; }
            set { m_GeometryCached = value; }
        }

        internal int splineHashCode
        {
            get { return m_ActiveSplineHash; }
        }

        internal Sprite[] spriteArray
        {
            get { return m_SpriteArray; }
        }

        internal SpriteShapeParameters spriteShapeParameters
        {
            get { return m_ActiveShapeParameters; }
        }

        internal SpriteShapeGeometryCache spriteShapeGeometryCache
        {
            get
            {
                if (!m_SpriteShapeGeometryCache)
                    m_SpriteShapeGeometryCache = GetComponent<SpriteShapeGeometryCache>();
                return m_SpriteShapeGeometryCache;
            }
        }

        public int spriteShapeHashCode
        {
            get { return m_ActiveSpriteShapeHash; }
        }

        public bool worldSpaceUVs
        {
            get { return m_WorldSpaceUV; }
            set { m_WorldSpaceUV = value; }
        }

        public float fillPixelsPerUnit
        {
            get { return m_FillPixelPerUnit; }
            set { m_FillPixelPerUnit = value; }
        }

        public bool enableTangents
        {
            get { return m_EnableTangents; }
            set { m_EnableTangents = value; }
        }

        public float stretchTiling
        {
            get { return m_StretchTiling; }
            set { m_StretchTiling = value; }
        }

        public int splineDetail
        {
            get { return m_SplineDetail; }
            set { m_SplineDetail = Mathf.Max(0, value); }
        }

        public int colliderDetail
        {
            get { return m_ColliderDetail; }
            set { m_ColliderDetail = Mathf.Max(0, value); }
        }

        public float colliderOffset
        {
            get { return m_ColliderOffset; }
            set { m_ColliderOffset = value; }
        }

        public float cornerAngleThreshold
        {
            get { return m_CornerAngleThreshold; }
            set { m_CornerAngleThreshold = value; }
        }

        public bool autoUpdateCollider
        {
            get { return m_UpdateCollider; }
            set { m_UpdateCollider = value; }
        }

        public bool optimizeCollider
        {
            get { return m_OptimizeCollider; }
        }

        internal bool optimizeColliderInternal
        {
            set { m_OptimizeCollider = value; }
        }
        
        public bool optimizeGeometry
        {
            get { return m_OptimizeGeometry; }
        }

        public bool hasCollider
        {
            get { return (edgeCollider != null) || (polygonCollider != null); }
        }

        public Spline spline
        {
            get { return m_Spline; }
        }

        public SpriteShape spriteShape
        {
            get { return m_SpriteShape; }
            set { m_SpriteShape = value; }
        }

        public EdgeCollider2D edgeCollider
        {
            get
            {
                if (!m_EdgeCollider2D)
                    m_EdgeCollider2D = GetComponent<EdgeCollider2D>();
                return m_EdgeCollider2D;
            }
        }

        public PolygonCollider2D polygonCollider
        {
            get
            {
                if (!m_PolygonCollider2D)
                    m_PolygonCollider2D = GetComponent<PolygonCollider2D>();
                return m_PolygonCollider2D;
            }
        }

        public SpriteShapeRenderer spriteShapeRenderer
        {
            get
            {
                if (!m_SpriteShapeRenderer)
                    m_SpriteShapeRenderer = GetComponent<SpriteShapeRenderer>();
                return m_SpriteShapeRenderer;
            }
        }

        #endregion

        #region EventHandles.

        void DisposeInternal()
        {
            if (m_ColliderData.IsCreated)
                m_ColliderData.Dispose();
            if (m_TangentData.IsCreated)
                m_TangentData.Dispose();
        }

        void OnApplicationQuit()
        {
            DisposeInternal();
        }

        void OnEnable()
        {
            m_DynamicOcclusionOverriden = true;
            m_DynamicOcclusionLocal = spriteShapeRenderer.allowOcclusionWhenDynamic;
            spriteShapeRenderer.allowOcclusionWhenDynamic = false;
            UpdateSpriteData();
        }

        void OnDisable()
        {
            DisposeInternal();
        }

        void OnDestroy()
        {

        }

        void Reset()
        {
            m_SplineDetail = (int)QualityDetail.High;
            m_AdaptiveUV = true;
            m_StretchUV = false;
            m_FillPixelPerUnit = 100f;

            m_ColliderDetail = (int)QualityDetail.High;
            m_StretchTiling = 1.0f;
            m_WorldSpaceUV = false;
            m_CornerAngleThreshold = 30.0f;
            m_ColliderOffset = 0;
            m_UpdateCollider = true;
            m_OptimizeCollider = true;
            m_OptimizeGeometry = true;
            m_EnableTangents = false;

            spline.Clear();
            spline.InsertPointAt(0, Vector2.left + Vector2.down);
            spline.InsertPointAt(1, Vector2.left + Vector2.up);
            spline.InsertPointAt(2, Vector2.right + Vector2.up);
            spline.InsertPointAt(3, Vector2.right + Vector2.down);
        }

        static void SmartDestroy(UnityEngine.Object o)
        {
            if (o == null)
                return;

#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(o);
            else
#endif
                Destroy(o);
        }

        #endregion

        #region HashAndDataCheck

        public void RefreshSpriteShape()
        {
            m_ActiveSplineHash = 0;
        }

        // Ensure Neighbor points are not too close to each other.
        bool ValidateSpline()
        {
            int pointCount = spline.GetPointCount();
            if (pointCount < 2)
                return false;
            for (int i = 0; i < pointCount - 1; ++i)
            {
                var vec = spline.GetPosition(i) - spline.GetPosition(i + 1);
                if (vec.sqrMagnitude < s_DistanceTolerance)
                {
                    Debug.LogWarningFormat(gameObject, "[SpriteShape] Control points {0} & {1} are too close. SpriteShape will not be generated for < {2} >.", i, i + 1, gameObject.name);
                    return false;
                }
            }
            return true;
        }

        // Ensure SpriteShape is valid if not
        bool ValidateSpriteShapeTexture()
        {
            bool valid = false;
            
            // Check if SpriteShape Profile is valid.
            if (spriteShape != null)
            {
                // Open ended and no valid Sprites set. Check if it has a valid fill texture.
                if (!spline.isOpenEnded)
                {
                    valid = (spriteShape.fillTexture != null);
                }
            }
            else
            {
                // Warn that no SpriteShape is set.
                Debug.LogWarningFormat(gameObject, "[SpriteShape] A valid SpriteShape profile has not been set for gameObject < {0} >.", gameObject.name);
#if UNITY_EDITOR
            // We allow null SpriteShape for rapid prototyping in Editor.
            valid = true;
#endif
            }                
            return valid;
        }

        internal bool ValidateUTess2D()
        {
            bool uTess2D = m_UTess2D;
            // Check for all properties that can create overlaps/intersections.
            if (m_UTess2D && null != spriteShape)
            {
                uTess2D = (spriteShape.fillOffset == 0);
            }
            return uTess2D;
        }

        bool HasSpriteShapeChanged()
        {
            bool changed = (m_ActiveSpriteShape != spriteShape);
            if (changed)
                m_ActiveSpriteShape = spriteShape;
            return changed;
        }

        bool HasSpriteShapeDataChanged()
        {
            bool updateSprites = HasSpriteShapeChanged();
            if (spriteShape)
            {
                var hashCode = SpriteShape.GetSpriteShapeHashCode(spriteShape);
                if (spriteShapeHashCode != hashCode)
                {
                    m_ActiveSpriteShapeHash = hashCode;
                    updateSprites = true;
                }
            }
            return updateSprites;
        }

        bool HasSplineDataChanged()
        {
            unchecked
            {
                // Spline.
                int hashCode = (int)2166136261 ^ spline.GetHashCode();

                // Local Stuff.
                hashCode = hashCode * 16777619 ^ (m_UTess2D ? 1 : 0);
                hashCode = hashCode * 16777619 ^ (m_WorldSpaceUV ? 1 : 0); 
                hashCode = hashCode * 16777619 ^ (m_EnableTangents ? 1 : 0);
                hashCode = hashCode * 16777619 ^ (m_GeometryCached ? 1 : 0);
                hashCode = hashCode * 16777619 ^ (m_OptimizeGeometry ? 1 : 0);
                hashCode = hashCode * 16777619 ^ (m_StretchTiling.GetHashCode());
                hashCode = hashCode * 16777619 ^ (m_ColliderOffset.GetHashCode());
                hashCode = hashCode * 16777619 ^ (m_ColliderDetail.GetHashCode());

                if (splineHashCode != hashCode)
                {
                    m_ActiveSplineHash = hashCode;
                    return true;
                }
            }
            return false;
        }

        void LateUpdate()
        {
            BakeCollider();
        }

        void OnWillRenderObject()
        {
            BakeMesh();
        }

        /// <summary>
        /// Generate geometry on a Job. 
        /// </summary>
        /// <returns>JobHandle for the SpriteShape geometry generation job.</returns>
        public JobHandle BakeMesh()
        {
            JobHandle jobHandle = default;

#if !UNITY_EDITOR            
            if (spriteShapeGeometryCache)
            {
                // If SpriteShapeGeometry has already been uploaded, don't bother checking further.
                if (0 != m_ActiveSplineHash && 0 != spriteShapeGeometryCache.maxArrayCount)
                    return jobHandle;
            }
#endif                

            bool valid = ValidateSpline();

            if (valid)
            {
                bool splineChanged = HasSplineDataChanged();
                bool spriteShapeChanged = HasSpriteShapeDataChanged();
                bool spriteShapeParametersChanged = UpdateSpriteShapeParameters();

                if (spriteShapeChanged)
                {
                    UpdateSpriteData();
                }

                if (splineChanged || spriteShapeChanged || spriteShapeParametersChanged)
                {
                    jobHandle = ScheduleBake();
                }
                
#if UNITY_EDITOR
                if (spriteShapeChanged && spriteShapeGeometryCache && geometryCached)
                {
                    jobHandle.Complete();
                    spriteShapeGeometryCache.UpdateGeometryCache();
                }
#endif                
            }
            return jobHandle;
        }

#endregion

#region UpdateData

        /// <summary>
        /// Force update SpriteShape parameters. 
        /// </summary>
        /// <returns>Returns true if there are changes</returns>
        public bool UpdateSpriteShapeParameters()
        {
            bool carpet = !spline.isOpenEnded;
            bool smartSprite = true;
            bool adaptiveUV = m_AdaptiveUV;
            bool stretchUV = m_StretchUV;
            bool spriteBorders = false;
            uint fillScale = 0;
            uint splineDetail = (uint)m_SplineDetail;
            float borderPivot = 0f;
            float angleThreshold = (m_CornerAngleThreshold >= 0 && m_CornerAngleThreshold < 90) ? m_CornerAngleThreshold : 89.9999f;
            Texture2D fillTexture = null;
            Matrix4x4 transformMatrix = Matrix4x4.identity;

            if (spriteShape)
            {
                if (worldSpaceUVs)
                    transformMatrix = transform.localToWorldMatrix;

                fillTexture = spriteShape.fillTexture;
                fillScale = stretchUV ? (uint)stretchTiling : (uint)fillPixelsPerUnit;
                borderPivot = spriteShape.fillOffset;
                spriteBorders = spriteShape.useSpriteBorders;
                // If Corners are enabled, set smart-sprite to false.
                if (spriteShape.cornerSprites.Count > 0)
                    smartSprite = false;
            }
            else
            {
#if UNITY_EDITOR
                fillTexture = UnityEditor.EditorGUIUtility.whiteTexture;
                fillScale = 100;
#endif
            }

            bool changed = m_ActiveShapeParameters.adaptiveUV != adaptiveUV ||
                m_ActiveShapeParameters.angleThreshold != angleThreshold ||
                m_ActiveShapeParameters.borderPivot != borderPivot ||
                m_ActiveShapeParameters.carpet != carpet ||
                m_ActiveShapeParameters.fillScale != fillScale ||
                m_ActiveShapeParameters.fillTexture != fillTexture ||
                m_ActiveShapeParameters.smartSprite != smartSprite ||
                m_ActiveShapeParameters.splineDetail != splineDetail ||
                m_ActiveShapeParameters.spriteBorders != spriteBorders ||
                m_ActiveShapeParameters.transform != transformMatrix ||
                m_ActiveShapeParameters.stretchUV != stretchUV;

            m_ActiveShapeParameters.adaptiveUV = adaptiveUV;
            m_ActiveShapeParameters.stretchUV = stretchUV;
            m_ActiveShapeParameters.angleThreshold = angleThreshold;
            m_ActiveShapeParameters.borderPivot = borderPivot;
            m_ActiveShapeParameters.carpet = carpet;
            m_ActiveShapeParameters.fillScale = fillScale;
            m_ActiveShapeParameters.fillTexture = fillTexture;
            m_ActiveShapeParameters.smartSprite = smartSprite;
            m_ActiveShapeParameters.splineDetail = splineDetail;
            m_ActiveShapeParameters.spriteBorders = spriteBorders;
            m_ActiveShapeParameters.transform = transformMatrix;

            return changed;
        }

        void UpdateSpriteData()
        {
            if (spriteShape)
            {
                List<Sprite> edgeSpriteList = new List<Sprite>();
                List<Sprite> cornerSpriteList = new List<Sprite>();
                List<AngleRangeInfo> angleRangeInfoList = new List<AngleRangeInfo>();

                List<AngleRange> sortedAngleRanges = new List<AngleRange>(spriteShape.angleRanges);
                sortedAngleRanges.Sort((a, b) => a.order.CompareTo(b.order));

                for (int i = 0; i < sortedAngleRanges.Count; i++)
                {
                    bool validSpritesFound = false;
                    AngleRange angleRange = sortedAngleRanges[i];
                    foreach (Sprite edgeSprite in angleRange.sprites)
                    {
                        if (edgeSprite != null)
                        {
                            validSpritesFound = true;
                            break;
                        }
                    }

                    if (validSpritesFound)
                    {
                        AngleRangeInfo angleRangeInfo = new AngleRangeInfo();
                        angleRangeInfo.start = angleRange.start;
                        angleRangeInfo.end = angleRange.end;
                        angleRangeInfo.order = (uint)angleRange.order;
                        List<int> spriteIndices = new List<int>();
                        foreach (Sprite edgeSprite in angleRange.sprites)
                        {
                            edgeSpriteList.Add(edgeSprite);
                            spriteIndices.Add(edgeSpriteList.Count - 1);
                        }
                        angleRangeInfo.sprites = spriteIndices.ToArray();
                        angleRangeInfoList.Add(angleRangeInfo);
                    }
                }

                bool validCornerSpritesFound = false;
                foreach (CornerSprite cornerSprite in spriteShape.cornerSprites)
                {
                    if (cornerSprite.sprites[0] != null)
                    {
                        validCornerSpritesFound = true;
                        break;
                    }
                }

                if (validCornerSpritesFound)
                {
                    for (int i = 0; i < spriteShape.cornerSprites.Count; i++)
                    {
                        CornerSprite cornerSprite = spriteShape.cornerSprites[i];
                        cornerSpriteList.Add(cornerSprite.sprites[0]);
                    }
                }

                m_EdgeSpriteArray = edgeSpriteList.ToArray();
                m_CornerSpriteArray = cornerSpriteList.ToArray();
                m_AngleRangeInfoArray = angleRangeInfoList.ToArray();

                List<Sprite> spriteList = new List<Sprite>();
                spriteList.AddRange(m_EdgeSpriteArray);
                spriteList.AddRange(m_CornerSpriteArray);
                m_SpriteArray = spriteList.ToArray();
            }
            else
            {
                m_SpriteArray = new Sprite[0];
                m_EdgeSpriteArray = new Sprite[0];
                m_CornerSpriteArray = new Sprite[0];
                m_AngleRangeInfoArray = new AngleRangeInfo[0];
            }
        }

        NativeArray<ShapeControlPoint> GetShapeControlPoints()
        {
            int pointCount = spline.GetPointCount();
            NativeArray<ShapeControlPoint> shapePoints = new NativeArray<ShapeControlPoint>(pointCount, Allocator.Temp);
            for (int i = 0; i < pointCount; ++i)
            {
                ShapeControlPoint shapeControlPoint;
                shapeControlPoint.position = spline.GetPosition(i);
                shapeControlPoint.leftTangent = spline.GetLeftTangent(i);
                shapeControlPoint.rightTangent = spline.GetRightTangent(i);
                shapeControlPoint.mode = (int)spline.GetTangentMode(i);
                shapePoints[i] = shapeControlPoint;
            }
            return shapePoints;
        }

        NativeArray<SplinePointMetaData> GetSplinePointMetaData()
        {
            int pointCount = spline.GetPointCount();
            NativeArray<SplinePointMetaData> shapeMetaData = new NativeArray<SplinePointMetaData>(pointCount, Allocator.Temp);
            for (int i = 0; i < pointCount; ++i)
            {                
                SplinePointMetaData metaData;
                metaData.height = m_Spline.GetHeight(i);
                metaData.spriteIndex = (uint)m_Spline.GetSpriteIndex(i);
                metaData.cornerMode = (int)m_Spline.GetCornerMode(i);
                shapeMetaData[i] = metaData;
            }
            return shapeMetaData;
        }

        int CalculateMaxArrayCount(NativeArray<ShapeControlPoint> shapePoints)
        {
            int maxVertexCount = 1024 * 64;
            bool hasSprites = false;
            float smallestWidth = 99999.0f;

            if (null != spriteArray)
            {
                foreach (var sprite in m_SpriteArray)
                {
                    if (sprite != null)
                    {
                        hasSprites = true;
                        float pixelWidth = BezierUtility.GetSpritePixelWidth(sprite);
                        smallestWidth = (smallestWidth > pixelWidth) ? pixelWidth : smallestWidth;
                    }
                }
            }

            // Approximate vertex Array Count. Include Corners and Wide Sprites into account.
            float smallestSegment = smallestWidth;
            float shapeLength = BezierUtility.BezierLength(shapePoints, splineDetail, ref smallestSegment) * 4.0f;
            int adjustShape = shapePoints.Length * 4 * splineDetail;
            int adjustWidth = hasSprites ? ((int)(shapeLength / smallestSegment) * splineDetail) + adjustShape : 0;
            adjustShape = optimizeGeometry ? (adjustShape) : (adjustShape * 2);
            adjustShape = ValidateSpriteShapeTexture() ? adjustShape : 0;
            maxArrayCount = adjustShape + adjustWidth;
            maxArrayCount = math.min(maxArrayCount, maxVertexCount);
            return maxArrayCount;
        }

#endregion

        unsafe JobHandle ScheduleBake()
        {
            JobHandle jobHandle = default;

            bool staticUpload = Application.isPlaying;
#if !UNITY_EDITOR
            staticUpload = true;
#endif
            if (staticUpload && geometryCached)
            {
                if (spriteShapeGeometryCache)
                    if (spriteShapeGeometryCache.maxArrayCount != 0)
                        return spriteShapeGeometryCache.Upload(spriteShapeRenderer, this);
            }

            int pointCount = spline.GetPointCount();
            NativeArray<ShapeControlPoint> shapePoints = GetShapeControlPoints();
            NativeArray<SplinePointMetaData> shapeMetaData = GetSplinePointMetaData();
            maxArrayCount = CalculateMaxArrayCount(shapePoints);

            if (maxArrayCount > 0 && enabled)
            {
                // Collider Data
                if (m_ColliderData.IsCreated)
                    m_ColliderData.Dispose();
                m_ColliderData = new NativeArray<float2>(maxArrayCount, Allocator.Persistent);

                // Tangent Data
                if (!m_TangentData.IsCreated)
                    m_TangentData = new NativeArray<Vector4>(1, Allocator.Persistent);

                NativeArray<ushort> indexArray;
                NativeSlice<Vector3> posArray;
                NativeSlice<Vector2> uv0Array;
                NativeArray<Bounds> bounds = spriteShapeRenderer.GetBounds();
                NativeArray<SpriteShapeSegment> geomArray = spriteShapeRenderer.GetSegments(shapePoints.Length * 8);
                NativeSlice<Vector4> tanArray = new NativeSlice<Vector4>(m_TangentData);

                if (m_EnableTangents)
                { 
                    spriteShapeRenderer.GetChannels(maxArrayCount, out indexArray, out posArray, out uv0Array, out tanArray);
                }
                else
                {
                    spriteShapeRenderer.GetChannels(maxArrayCount, out indexArray, out posArray, out uv0Array);
                }

                var uTess2D = ValidateUTess2D();
                var spriteShapeJob = new SpriteShapeGenerator() { m_Bounds = bounds, m_PosArray = posArray, m_Uv0Array = uv0Array, m_TanArray = tanArray, m_GeomArray = geomArray, m_IndexArray = indexArray, m_ColliderPoints = m_ColliderData };
                spriteShapeJob.Prepare(this, m_ActiveShapeParameters, maxArrayCount, shapePoints, shapeMetaData, m_AngleRangeInfoArray, m_EdgeSpriteArray, m_CornerSpriteArray, uTess2D);
                // Only update Handle for an actual Job is scheduled.
                m_JobHandle = spriteShapeJob.Schedule();
                spriteShapeRenderer.Prepare(m_JobHandle, m_ActiveShapeParameters, m_SpriteArray);
                jobHandle = m_JobHandle;

#if UNITY_EDITOR
                if (spriteShapeGeometryCache && geometryCached)
                    spriteShapeGeometryCache.SetGeometryCache(maxArrayCount, posArray, uv0Array, tanArray, indexArray, geomArray);
#endif

                JobHandle.ScheduleBatchedJobs();
            }

            if (m_DynamicOcclusionOverriden)
            {
                spriteShapeRenderer.allowOcclusionWhenDynamic = m_DynamicOcclusionLocal;
                m_DynamicOcclusionOverriden = false;
            }
            shapePoints.Dispose();
            shapeMetaData.Dispose();
            return jobHandle;
        }

        public void BakeCollider()
        {
            // Previously this must be explicitly called if using BakeMesh.
            // But now we do it internally. BakeCollider_CanBeCalledMultipleTimesWithoutJobComplete
            m_JobHandle.Complete();
            
            if (m_ColliderData.IsCreated)
            {
                if (autoUpdateCollider)
                {
                    if (hasCollider)
                    {
                        int maxCount = short.MaxValue - 1;
                        float2 last = (float2)0;
                        List<Vector2> m_ColliderSegment = new List<Vector2>();
                        for (int i = 0; i < maxCount; ++i)
                        {
                            float2 now = m_ColliderData[i];
                            if (!math.any(last) && !math.any(now))
                                break;
                            m_ColliderSegment.Add(new Vector2(now.x, now.y));
                        }

                        if (edgeCollider != null)
                            edgeCollider.points = m_ColliderSegment.ToArray();
                        if (polygonCollider != null)
                            polygonCollider.points = m_ColliderSegment.ToArray();
#if UNITY_EDITOR
                        UnityEditor.SceneView.RepaintAll();
#endif                        
                    }
                }
                // Dispose Collider as its no longer needed.
                m_ColliderData.Dispose();
            }
        }

        internal void BakeMeshForced()
        {
            if (spriteShapeRenderer != null)
            {
                var hasSplineChanged = HasSplineDataChanged();
                if (!spriteShapeRenderer.isVisible && hasSplineChanged)
                {
                    BakeMesh();
                    Rendering.CommandBuffer rc = new Rendering.CommandBuffer();
                    rc.GetTemporaryRT(0, 256, 256, 0);
                    rc.SetRenderTarget(0);
                    rc.DrawRenderer(spriteShapeRenderer, spriteShapeRenderer.sharedMaterial);
                    rc.ReleaseTemporaryRT(0);
                    Graphics.ExecuteCommandBuffer(rc);
                }
            }
        }

        Texture2D GetTextureFromIndex(int index)
        {
            if (index == 0)
                return spriteShape ? spriteShape.fillTexture : null;

            --index;
            if (index < m_EdgeSpriteArray.Length)
                return GetSpriteTexture(m_EdgeSpriteArray[index]);

            index -= m_EdgeSpriteArray.Length;
            return GetSpriteTexture(m_CornerSpriteArray[index]);
        }

        Texture2D GetSpriteTexture(Sprite sprite)
        {
            if (sprite)
            {
#if UNITY_EDITOR
                return UnityEditor.Sprites.SpriteUtility.GetSpriteTexture(sprite, sprite.packed);
#else
                return sprite.texture;
#endif
            }

            return null;
        }
    }
}
