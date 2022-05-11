using UnityEditor;
using UnityEngine;
using UnityEditor.Sprites;
using System.Collections;
using System.Collections.Generic;

namespace UnityEditor.U2D
{
    public static class EditorSpriteGUIUtility
    {
        public enum FitMode
        {
            BestFit,
            FitHorizontal,
            FitVertical,
            Fill,
            Tiled
        }

        private static Material s_SpriteMaterial;
        public static Material spriteMaterial
        {
            get
            {
                if (s_SpriteMaterial == null)
                {
                    s_SpriteMaterial = new Material(Shader.Find("Hidden/InternalSpritesInspector"));
                    s_SpriteMaterial.hideFlags = HideFlags.DontSave;
                }
                s_SpriteMaterial.SetFloat("_AdjustLinearForGamma", PlayerSettings.colorSpace == ColorSpace.Linear ? 1.0f : 0.0f);
                return s_SpriteMaterial;
            }
        }

        public static Texture GetOriginalSpriteTexture(Sprite sprite)
        {
            return UnityEditor.Sprites.SpriteUtility.GetSpriteTexture(sprite, false);
        }

        public static Vector2[] GetOriginalSpriteUvs(Sprite sprite)
        {
            return UnityEditor.Sprites.SpriteUtility.GetSpriteUVs(sprite, false);
        }

        public static void DrawSpriteInRectPrepare(Rect rect, Sprite sprite, FitMode fitMode, bool excludeBorders, bool forceQuad, Mesh mesh)
        {
            var vertices = new List<Vector3>();
            var uvs = new List<Vector2>();
            var indices = new List<int>();
            mesh.Clear();

            if (!sprite)
            {
                mesh.UploadMeshData(false);
                return;
            }

            Vector2 scale = Vector2.one;
            Rect spriteRect = sprite.textureRect;
            Vector2 bottomLeftBorderOffset = sprite.rect.position + new Vector2(sprite.border.x, sprite.border.y) - spriteRect.position;
            Vector2 topRightBorderOffset = new Vector2(sprite.border.z, sprite.border.w) + (spriteRect.position + spriteRect.size) - (sprite.rect.position + sprite.rect.size);

            if (excludeBorders)
            {
                forceQuad = true;
                spriteRect.position = spriteRect.position + bottomLeftBorderOffset;
                spriteRect.size = spriteRect.size - bottomLeftBorderOffset - topRightBorderOffset;
            }

            bool tiled = false;

            if (fitMode == FitMode.Tiled)
            {
                tiled = true;
                forceQuad = true;
                fitMode = FitMode.BestFit;
            }

            if (fitMode == FitMode.BestFit)
            {
                float spriteRatio = spriteRect.width / spriteRect.height;
                float frameRatio = rect.width / rect.height;

                if (spriteRatio < frameRatio)
                    fitMode = FitMode.FitVertical;
                else
                    fitMode = FitMode.FitHorizontal;
            }

            if (fitMode == FitMode.FitHorizontal)
                scale = Vector2.one * (rect.width / spriteRect.width);

            if (fitMode == FitMode.FitVertical)
                scale = Vector2.one * (rect.height / spriteRect.height);

            if (fitMode == FitMode.Fill)
            {
                scale.x = rect.width / spriteRect.width;
                scale.y = rect.height / spriteRect.height;
            }

            Texture spriteTexture = GetOriginalSpriteTexture(sprite);
            if (spriteTexture == null)
                return;

            if (forceQuad)
            {
                Vector2 uvScale = new Vector2(1f / spriteTexture.width, 1f / spriteTexture.height);
                Vector2 uvPos = Vector2.Scale(spriteRect.position, uvScale);
                Vector2 uvSize = Vector2.Scale(spriteRect.size, uvScale);
                Vector2 uv0 = uvPos;
                Vector2 uv1 = uvPos + Vector2.up * uvSize.y;
                Vector2 uv2 = uvPos + Vector2.right * uvSize.x;
                Vector2 uv3 = uvPos + uvSize;
                Vector3 v0 = new Vector3(uv0.x * spriteTexture.width - spriteRect.position.x - spriteRect.width * 0.5f, uv0.y * spriteTexture.height - spriteRect.position.y - spriteRect.height * 0.5f, 0f);
                Vector3 v1 = new Vector3(uv1.x * spriteTexture.width - spriteRect.position.x - spriteRect.width * 0.5f, uv1.y * spriteTexture.height - spriteRect.position.y - spriteRect.height * 0.5f, 0f);
                Vector3 v2 = new Vector3(uv2.x * spriteTexture.width - spriteRect.position.x - spriteRect.width * 0.5f, uv2.y * spriteTexture.height - spriteRect.position.y - spriteRect.height * 0.5f, 0f);
                Vector3 v3 = new Vector3(uv3.x * spriteTexture.width - spriteRect.position.x - spriteRect.width * 0.5f, uv3.y * spriteTexture.height - spriteRect.position.y - spriteRect.height * 0.5f, 0f);
                v0 = Vector3.Scale(v0, scale);
                v1 = Vector3.Scale(v1, scale);
                v2 = Vector3.Scale(v2, scale);
                v3 = Vector3.Scale(v3, scale);

                //TODO: Support vertical tiling when horizontal fitted
                if (tiled && fitMode == FitMode.FitVertical)
                {
                    Vector2 scaledRectSize = Vector2.Scale(rect.size, new Vector2(1f / scale.x, 1f / scale.y));
                    float halfDistanceToFill = (scaledRectSize.x - spriteRect.width) * 0.5f;
                    int halfFillSegmentCount = (int)Mathf.Ceil(halfDistanceToFill / spriteRect.width);
                    int segmentCount = halfFillSegmentCount * 2 + 1;
                    int vertexCount = segmentCount * 4;

                    vertices.Capacity = vertexCount;
                    uvs.Capacity = vertexCount;
                    indices.Capacity = vertexCount;

                    Vector3 offset = Vector3.zero;
                    Vector3 offsetStep = Vector3.Scale(Vector3.right * spriteRect.width, scale);

                    float distanceStep = spriteRect.width;
                    float distanceToFill = halfDistanceToFill + distanceStep;

                    int vertexIndex = 0;

                    for (int i = 0; i <= halfFillSegmentCount; ++i)
                    {
                        float t = Mathf.Clamp01(distanceToFill / spriteRect.width);

                        uvs.Add(uv0);
                        uvs.Add(uv1);
                        uvs.Add(Vector3.Lerp(uv0, uv2, t));
                        uvs.Add(Vector3.Lerp(uv1, uv3, t));

                        vertices.Add(v0 + offset);
                        vertices.Add(v1 + offset);
                        vertices.Add(Vector3.Lerp(v0, v2, t) + offset);
                        vertices.Add(Vector3.Lerp(v1, v3, t) + offset);

                        indices.Add(vertexIndex);
                        indices.Add(vertexIndex + 2);
                        indices.Add(vertexIndex + 1);
                        indices.Add(vertexIndex + 2);
                        indices.Add(vertexIndex + 3);
                        indices.Add(vertexIndex + 1);

                        vertexIndex += 4;

                        if (i > 0)
                        {
                            uvs.Add(Vector2.Lerp(uv0, uv2, 1f - t));
                            uvs.Add(Vector2.Lerp(uv1, uv3, 1f - t));
                            uvs.Add(uv2);
                            uvs.Add(uv3);

                            vertices.Add(Vector3.Lerp(v0, v2, 1f - t) - offset);
                            vertices.Add(Vector3.Lerp(v1, v3, 1f - t) - offset);
                            vertices.Add(v2 - offset);
                            vertices.Add(v3 - offset);

                            indices.Add(vertexIndex);
                            indices.Add(vertexIndex + 2);
                            indices.Add(vertexIndex + 1);
                            indices.Add(vertexIndex + 2);
                            indices.Add(vertexIndex + 3);
                            indices.Add(vertexIndex + 1);

                            vertexIndex += 4;
                        }

                        offset += offsetStep;
                        distanceToFill -= distanceStep;
                    }
                }
                else
                {
                    vertices.AddRange(new Vector3[] { v0, v1, v2, v3 });
                    uvs.AddRange(new Vector2[] { uv0, uv1, uv2, uv3 });
                    indices.AddRange(new int[] { 0, 2, 1, 2, 3, 1 });
                }
            }
            else
            {
                ushort[] triangles = sprite.triangles;
                indices.Capacity = triangles.Length;

                for (int i = 0; i < triangles.Length; ++i)
                    indices.Add((int)triangles[i]);

                uvs.AddRange(GetOriginalSpriteUvs(sprite));
                vertices.Capacity = uvs.Count;

                for (int i = 0; i < uvs.Count; ++i)
                {
                    Vector3 v = new Vector3(uvs[i].x * spriteTexture.width - spriteRect.position.x - spriteRect.width * 0.5f, uvs[i].y * spriteTexture.height - spriteRect.position.y - spriteRect.height * 0.5f, 0f);
                    vertices.Add(Vector3.Scale(v, scale));
                }
            }

            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(indices, 0);
            mesh.UploadMeshData(false);
        }

        public static void DrawMesh(Mesh mesh, Material material, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetTRS(position, rotation, scale);
            material.SetPass(0);
            Graphics.DrawMeshNow(mesh, matrix);
        }
    }
}
