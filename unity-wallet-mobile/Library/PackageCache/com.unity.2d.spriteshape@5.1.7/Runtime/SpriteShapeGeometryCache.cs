using System;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.U2D;
using Unity.Collections;

[Serializable]
internal struct SpriteShapeGeometryInfo
{
    [SerializeField]
    internal int geomIndex;
    [SerializeField]
    internal int indexCount;
    [SerializeField]
    internal int vertexCount;
    [SerializeField]
    internal int spriteIndex;
}

// Simple Cache for SpriteShape Geometry Data.
[AddComponentMenu("")]
internal class SpriteShapeGeometryCache : MonoBehaviour
{

    // Serialized Data.
    [SerializeField]
    [HideInInspector]
    int m_MaxArrayCount;
    [SerializeField]
    [HideInInspector]
    Vector3[] m_PosArray = null;
    [SerializeField]
    [HideInInspector]
    Vector2[] m_Uv0Array = null;
    [SerializeField]
    [HideInInspector]
    Vector4[] m_TanArray = null;
    [SerializeField]
    [HideInInspector]
    ushort[] m_IndexArray = null;
    [SerializeField]
    [HideInInspector]
    SpriteShapeGeometryInfo[] m_GeomArray = null;

    // Update set.
    bool m_RequiresUpdate = false;
    bool m_RequiresUpload = false;
    NativeSlice<Vector3> m_PosArrayCache;
    NativeSlice<Vector2> m_Uv0ArrayCache;
    NativeSlice<Vector4> m_TanArrayCache;
    NativeArray<ushort> m_IndexArrayCache;
    NativeArray<UnityEngine.U2D.SpriteShapeSegment> m_GeomArrayCache;

    internal ushort[] indexArray
    {
        get { return m_IndexArray; }
    }
    internal int maxArrayCount
    {
        get { return m_MaxArrayCount; }
    }

    void OnEnable()
    {
        m_RequiresUpload = true;
        m_RequiresUpdate = false;
    }

    // Set Geometry Cache.
    internal void SetGeometryCache(int _maxArrayCount, NativeSlice<Vector3> _posArray, NativeSlice<Vector2> _uv0Array, NativeSlice<Vector4> _tanArray, NativeArray<ushort> _indexArray, NativeArray<UnityEngine.U2D.SpriteShapeSegment> _geomArray)
    {
        m_RequiresUpdate = true;
        m_PosArrayCache = _posArray;
        m_Uv0ArrayCache = _uv0Array;
        m_TanArrayCache = _tanArray;
        m_GeomArrayCache = _geomArray;
        m_IndexArrayCache = _indexArray;
        m_MaxArrayCount = _maxArrayCount;
    }

    // Update GeometryCache.
    internal void UpdateGeometryCache()
    {
        bool updateCache = m_RequiresUpdate && m_GeomArrayCache.IsCreated;
        updateCache = updateCache && m_IndexArrayCache.IsCreated;
        if (updateCache)
        {
            int geomCount = 0;
            int indexCount = 0;
            int vertexCount = 0;

            for (int i = 0; (i < m_GeomArrayCache.Length); ++i)
            {
                var geom = m_GeomArrayCache[i];
                indexCount += geom.indexCount;
                vertexCount += geom.vertexCount;
                if (geom.vertexCount > 0)
                    geomCount = i + 1;
            }

            m_GeomArray = new SpriteShapeGeometryInfo[geomCount];
            NativeArray<SpriteShapeGeometryInfo> geomInfoArray = m_GeomArrayCache.Reinterpret<SpriteShapeGeometryInfo>();
            SpriteShapeCopyUtility<SpriteShapeGeometryInfo>.Copy(m_GeomArray, geomInfoArray, geomCount);

            m_PosArray = new Vector3[vertexCount];
            m_Uv0Array = new Vector2[vertexCount];
            m_TanArray = new Vector4[vertexCount];
            m_IndexArray = new ushort[indexCount];

            SpriteShapeCopyUtility<ushort>.Copy(m_IndexArray, m_IndexArrayCache, indexCount);
            SpriteShapeCopyUtility<Vector3>.Copy(m_PosArray, m_PosArrayCache, vertexCount);
            SpriteShapeCopyUtility<Vector2>.Copy(m_Uv0Array, m_Uv0ArrayCache, vertexCount);
            if (m_TanArrayCache.Length >= vertexCount)
                SpriteShapeCopyUtility<Vector4>.Copy(m_TanArray, m_TanArrayCache, vertexCount);

            m_MaxArrayCount = (vertexCount > indexCount) ? vertexCount : indexCount;
            m_RequiresUpdate = false;
        }
    }

    internal JobHandle Upload(SpriteShapeRenderer sr, SpriteShapeController sc)
    {

        JobHandle jobHandle = (default);
        if (m_RequiresUpload)
        {

            // Update Geometries.
            NativeArray<SpriteShapeSegment> geomArray = sr.GetSegments(m_GeomArray.Length);
            NativeArray<SpriteShapeGeometryInfo> geomInfoArray = geomArray.Reinterpret<SpriteShapeGeometryInfo>();
            geomInfoArray.CopyFrom(m_GeomArray);

            // Update Mesh Data.
            NativeSlice<Vector3> posArray;
            NativeSlice<Vector2> uv0Array;
            NativeArray<ushort> indexArray;

            if (sc.enableTangents)
            {
                NativeSlice<Vector4> tanArray;
                sr.GetChannels(m_MaxArrayCount, out indexArray, out posArray, out uv0Array, out tanArray);
                SpriteShapeCopyUtility<Vector4>.Copy(tanArray, m_TanArray, m_TanArray.Length);
            }
            else
            {
                sr.GetChannels(m_MaxArrayCount, out indexArray, out posArray, out uv0Array);
            }

            SpriteShapeCopyUtility<Vector3>.Copy(posArray, m_PosArray, m_PosArray.Length);
            SpriteShapeCopyUtility<Vector2>.Copy(uv0Array, m_Uv0Array, m_Uv0Array.Length);
            SpriteShapeCopyUtility<ushort>.Copy(indexArray, m_IndexArray, m_IndexArray.Length);
            sr.Prepare(jobHandle, sc.spriteShapeParameters, sc.spriteArray);
            m_RequiresUpload = false;

        }
        return jobHandle;

    }

}
