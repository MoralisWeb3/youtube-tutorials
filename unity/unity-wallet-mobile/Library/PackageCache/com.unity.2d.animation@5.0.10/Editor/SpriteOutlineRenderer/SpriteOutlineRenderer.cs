using System.Collections.Generic;
using UnityEditor.U2D.Sprites;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal class SpriteOutlineRenderer
    {
        class OutlineRenderTexture
        {
            public Texture outlineTexture;
            public bool dirty;
        }

        Material m_OutlineMaterial;
        Material m_BitMaskMaterial;
        Material m_EdgeOutlineMaterial;
        Mesh m_CircleMesh;
        Dictionary<string, OutlineRenderTexture> m_OutlineTextureCache = new Dictionary<string, OutlineRenderTexture>();
        SkinningEvents m_EventSystem;

        static readonly int k_OutlineColorProperty = Shader.PropertyToID("_OutlineColor");
        static readonly int k_OutlineSizeProperty = Shader.PropertyToID("_OutlineSize");
        static readonly int k_AdjustLinearForGammaProperty = Shader.PropertyToID("_AdjustLinearForGamma");

        const int k_ReferenceTextureSize = 1024;

        public SpriteOutlineRenderer(SkinningEvents eventSystem)
        {
            m_EdgeOutlineMaterial = new Material(Shader.Find("Hidden/2D-Animation-SpriteEdgeOutline")) { hideFlags = HideFlags.HideAndDontSave };
            m_BitMaskMaterial = new Material(Shader.Find("Hidden/2D-Animation-SpriteBitmask")) { hideFlags = HideFlags.HideAndDontSave };
            m_OutlineMaterial = new Material(Shader.Find("Hidden/2D-Animation-SpriteOutline")) { hideFlags = HideFlags.HideAndDontSave };
            
            m_EventSystem = eventSystem;
            m_EventSystem.meshPreviewChanged.AddListener(OnMeshPreviewChanged);
            m_EventSystem.selectedSpriteChanged.AddListener(OnSelectionChanged);
            m_CircleMesh = GenerateCircleMesh();
        }

        public void Dispose()
        {
            DestroyMaterialsAndMeshes();
            DestroyTextures();
            
            m_EventSystem.meshPreviewChanged.RemoveListener(OnMeshPreviewChanged);
            m_EventSystem.selectedSpriteChanged.RemoveListener(OnSelectionChanged);
        }

        internal void RenderSpriteOutline(ISpriteEditor spriteEditor, SpriteCache sprite)
        {
            if (spriteEditor == null || sprite == null)
                return;

            if (Event.current.type == EventType.Repaint)
            {
                if (SelectionOutlineSettings.selectedSpriteOutlineSize < 0.01f || SelectionOutlineSettings.outlineColor.a < 0.01f)
                    return;

                var mesh = GetMesh(sprite);
                if (mesh == null)
                    return;
                
                UnityEngine.Profiling.Profiler.BeginSample("SpriteOutlineRenderer::RenderSpriteOutline");

                var vertices = mesh.vertices;
                var edges = sprite.GetMesh().edges;
                var multMatrix = Handles.matrix * sprite.GetLocalToWorldMatrixFromMode();
                
                var texture = spriteEditor.GetDataProvider<ITextureDataProvider>().texture;
                var outlineSize = SelectionOutlineSettings.selectedSpriteOutlineSize;
                var outlineColor = SelectionOutlineSettings.outlineColor;
                var adjustForGamma = PlayerSettings.colorSpace == ColorSpace.Linear ? 1.0f : 0.0f;

                if (edges != null && edges.Count > 0 && vertices.Length > 0)
                {
                    var finalOutlineSize = outlineSize / spriteEditor.zoomLevel;
                    DrawEdgeOutline(edges, vertices, multMatrix, finalOutlineSize, outlineColor, adjustForGamma);
                }
                else // Fallback: Draw using the Sobel filter.
                {
                    var finalOutlineSize = Mathf.Max(texture.width, texture.height) * outlineSize / k_ReferenceTextureSize;
                    DrawMeshOutline(mesh, sprite, multMatrix, finalOutlineSize, outlineColor, adjustForGamma);
                }

                UnityEngine.Profiling.Profiler.EndSample();
            }
        }

        void DrawEdgeOutline(List<Edge> edges, Vector3[] vertices, Matrix4x4 multMatrix, float outlineSize, Color outlineColor, float adjustForGamma)
        {
            m_EdgeOutlineMaterial.SetColor(k_OutlineColorProperty, outlineColor);
            m_EdgeOutlineMaterial.SetFloat(k_AdjustLinearForGammaProperty, adjustForGamma);
            m_EdgeOutlineMaterial.SetPass(0);
            
            var edgeCount = edges.Count;
            var vertexCount = vertices.Length;

            GL.PushMatrix();
            GL.MultMatrix(multMatrix);
            GL.Begin(GL.QUADS);
            for (var i = 0; i < edgeCount; i++)
            {
                var currentEdge = edges[i];
                if (currentEdge.index1 < 0 || currentEdge.index2 < 0 || currentEdge.index1 >= vertexCount || currentEdge.index2 >= vertexCount)
                    continue;
                
                var start = vertices[edges[i].index1];
                var end = vertices[edges[i].index2];
                var direction = (end - start).normalized;
                var right = Vector3.Cross(Vector3.forward, direction) * outlineSize;

                GL.Vertex(start - right);
                GL.Vertex(start + right);
                GL.Vertex(end + right);
                GL.Vertex(end - right);
            }
            GL.End();
            GL.PopMatrix();
            
            for (var i = 0; i < edgeCount; i++)
            {
                var currentEdge = edges[i];
                if (currentEdge.index1 < 0 || currentEdge.index2 < 0 || currentEdge.index1 >= vertexCount || currentEdge.index2 >= vertexCount)
                    continue;
                
                var start = vertices[edges[i].index1];
                var end = vertices[edges[i].index2];

                Graphics.DrawMeshNow(m_CircleMesh, multMatrix * Matrix4x4.TRS(start, Quaternion.identity, Vector3.one * outlineSize));
                Graphics.DrawMeshNow(m_CircleMesh, multMatrix * Matrix4x4.TRS(end, Quaternion.identity, Vector3.one * outlineSize));
            }
        }
        
        void DrawMeshOutline(Mesh mesh, SpriteCache spriteCache, Matrix4x4 multMatrix, float outlineSize, Color outlineColor, float adjustForGamma)
        {
            TryRegenerateMaskTexture(spriteCache);
            
            m_OutlineMaterial.SetColor(k_OutlineColorProperty, outlineColor);
            m_OutlineMaterial.SetFloat(k_AdjustLinearForGammaProperty, adjustForGamma);
            m_OutlineMaterial.SetFloat(k_OutlineSizeProperty, outlineSize);
            m_OutlineMaterial.SetPass(0);
            
            GL.PushMatrix();
            GL.MultMatrix(multMatrix);

            var meshBoundsRect = new Rect(mesh.bounds.min.x, mesh.bounds.min.y, mesh.bounds.size.x, mesh.bounds.size.y);
            GL.Begin(GL.QUADS);
            GL.Color(Color.white);
            GL.TexCoord(new Vector3(0, 0, 0));
            GL.Vertex3(meshBoundsRect.xMin, meshBoundsRect.yMin, 0);

            GL.TexCoord(new Vector3(1, 0, 0));
            GL.Vertex3(meshBoundsRect.xMax, meshBoundsRect.yMin, 0);

            GL.TexCoord(new Vector3(1, 1, 0));
            GL.Vertex3(meshBoundsRect.xMax, meshBoundsRect.yMax, 0);

            GL.TexCoord(new Vector3(0, 1, 0));
            GL.Vertex3(meshBoundsRect.xMin, meshBoundsRect.yMax, 0);
            GL.End();
            GL.PopMatrix();
        }

        Texture GenerateOutlineTexture(SpriteCache spriteCache, RenderTexture reuseRT)
        {
            if (spriteCache == null)
                return null;

            var mesh = GetMesh(spriteCache);
            if (mesh == null || (int)mesh.bounds.size.x == 0 || (int)mesh.bounds.size.y == 0)
                return null;

            var bounds = mesh.bounds;
            UnityEngine.Profiling.Profiler.BeginSample("SpriteOutlineRenderer::GenerateOutlineTexture");

            if (reuseRT == null || reuseRT.width != (int)bounds.size.x || reuseRT.height != (int)bounds.size.y)
            {
                UnityEngine.Profiling.Profiler.BeginSample("SpriteOutlineRenderer::CreateRT");
                if(reuseRT != null)
                    Object.DestroyImmediate(reuseRT);
                reuseRT = new RenderTexture((int)bounds.size.x, (int)bounds.size.y, 24, RenderTextureFormat.ARGBHalf) { filterMode = FilterMode.Bilinear };
                UnityEngine.Profiling.Profiler.EndSample();
            }

            var oldRT = RenderTexture.active;
            Graphics.SetRenderTarget(reuseRT);
            m_BitMaskMaterial.SetPass(0);
            UnityEngine.Profiling.Profiler.BeginSample("SpriteOutlineRenderer::DrawMesh");
            GL.Clear(false, true, new Color(0, 0, 0, 0));
            GL.PushMatrix();
            GL.LoadOrtho();
            var h = bounds.size.y * 0.5f;
            var w = h * (bounds.size.x / bounds.size.y);
            GL.LoadProjectionMatrix(Matrix4x4.Ortho(-w, w, -h, h, -1, 1));
            GL.Begin(GL.QUADS);
            GL.Color(Color.white);
            Graphics.DrawMeshNow(mesh, Matrix4x4.Translate(-bounds.center));
            GL.End();
            GL.PopMatrix();

            Graphics.SetRenderTarget(oldRT);
            UnityEngine.Profiling.Profiler.EndSample();

            UnityEngine.Profiling.Profiler.EndSample();
            return reuseRT;
        }

        static Mesh GetMesh(SpriteCache sprite)
        {
            var meshPreview = sprite.GetMeshPreview();

            if (meshPreview == null)
                return null;

            return meshPreview.mesh.vertexCount > 0 ? meshPreview.mesh : meshPreview.defaultMesh;
        }

        static Mesh GenerateCircleMesh()
        {
            const int triangleVerts = 9;
            var verts = new Vector3[triangleVerts];
            for (var i = 1; i < verts.Length; i++)
            {
                verts[i] = Quaternion.Euler(0, 0, i * 360f / (verts.Length - 1)) * Vector3.up;
            }
            var indices = new int[(verts.Length - 1) * 3];
            var index = 0;
            for (var i = 1; i < triangleVerts; i++)
            {
                indices[index++] = 0;
                indices[index++] = i;
                indices[index++] = i + 1 < triangleVerts ? i + 1 : 1;
            }

            return new Mesh { vertices = verts, triangles = indices };
        }
        
        void OnMeshPreviewChanged(MeshPreviewCache mesh)
        {
            AddOrUpdateMaskTexture(mesh.sprite, true);
        }

        void OnSelectionChanged(SpriteCache spriteCache)
        {
            AddOrUpdateMaskTexture(spriteCache, false);
        }

        void DestroyMaterialsAndMeshes()
        {
            if(m_EdgeOutlineMaterial != null)
                Object.DestroyImmediate(m_EdgeOutlineMaterial);
            if (m_BitMaskMaterial != null)
                Object.DestroyImmediate(m_BitMaskMaterial);
            if (m_OutlineMaterial != null)
                Object.DestroyImmediate(m_OutlineMaterial);
            
            if(m_CircleMesh != null)
                Object.DestroyImmediate(m_CircleMesh);
        }
        
        void DestroyTextures()
        {
            if (m_OutlineTextureCache != null)
            {
                foreach (var value in m_OutlineTextureCache.Values)
                {
                    if (value != null && value.outlineTexture != null)
                        Object.DestroyImmediate(value.outlineTexture);
                }
                m_OutlineTextureCache.Clear();
            }
        }

        void AddOrUpdateMaskTexture(SpriteCache sprite, bool regenerate)
        {
            if (m_OutlineTextureCache != null && sprite != null)
            {
                if (!m_OutlineTextureCache.ContainsKey(sprite.id))
                    m_OutlineTextureCache.Add(sprite.id, new OutlineRenderTexture() {dirty = true});

                var outlineTextureCache = m_OutlineTextureCache[sprite.id];
                outlineTextureCache.dirty |= regenerate;
            }
        }

        void TryRegenerateMaskTexture(SpriteCache sprite)
        {
            var selectedSprite = sprite.skinningCache.selectedSprite;

            var outlineTextureCache = m_OutlineTextureCache[sprite.id];
            if (sprite == selectedSprite)
            {
                if (outlineTextureCache.dirty || outlineTextureCache.outlineTexture == null)
                {
                    outlineTextureCache.outlineTexture = GenerateOutlineTexture(sprite, (RenderTexture)outlineTextureCache.outlineTexture);
                    if (outlineTextureCache.outlineTexture != null)
                    {
                        outlineTextureCache.outlineTexture.hideFlags = HideFlags.HideAndDontSave;
                        outlineTextureCache.dirty = false;
                    }
                }
                m_OutlineMaterial.mainTexture = outlineTextureCache.outlineTexture;
            }
        }
    }
}
